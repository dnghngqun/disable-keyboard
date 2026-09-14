using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using InputInterceptorNS;

namespace LaptopKeyboardDisabler;

public class ConfigData
{
    public int LaptopDeviceId { get; set; } = -1;
    public bool AutoToggleOnExternal { get; set; } = false;
    public bool HotkeyEnabled { get; set; } = true;
    public bool IsDisabledOnStartup { get; set; } = false;
}

public class KeyboardDeviceItem
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsLikelyLaptop { get; set; }

    public override string ToString()
    {
        string badge = IsLikelyLaptop ? "[Phím Laptop]" : "[Phím Ngoài/USB]";
        return $"{badge} Device #{DeviceId} - {Name}";
    }
}

public class InterceptionManager : IDisposable
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "LaptopKeyboardDisabler",
        "config.json"
    );

    [StructLayout(LayoutKind.Sequential)]
    private struct InterceptionDeviceItem
    {
        public IntPtr Handle;
        public IntPtr Unempty;
    }

    private const uint GENERIC_READ = 0x80000000;
    private const uint OPEN_EXISTING = 3;
    private const uint IOCTL_SET_EVENT = 0x222040;

    private IntPtr _context = IntPtr.Zero;
    private bool _isCustomContext = false;
    private Thread? _workerThread;
    private volatile bool _isRunning = false;
    private volatile bool _isLaptopDisabled = false;
    private volatile bool _isIdentifying = false;

    private int _laptopDeviceId = -1;
    private ConfigData _config = new();

    public event Action<bool>? StatusChanged;
    public event Action<int, string>? DeviceIdentified;

    public bool IsDriverInstalled
    {
        get
        {
            try
            {
                return InputInterceptor.CheckDriverInstalled() ||
                       File.Exists(Path.Combine(Environment.SystemDirectory, @"drivers\keyboard.sys"));
            }
            catch
            {
                return File.Exists(Path.Combine(Environment.SystemDirectory, @"drivers\keyboard.sys"));
            }
        }
    }
    public bool IsActive => _isRunning;
    public bool IsLaptopDisabled => _isLaptopDisabled;
    public int LaptopDeviceId => _laptopDeviceId;
    public ConfigData Config => _config;

    public InterceptionManager()
    {
        LoadConfig();
    }

    public static bool InstallDriver()
    {
        bool res = InputInterceptor.InstallDriver();
        if (res)
        {
            Program.FixMouseUpperFilters();
        }
        return res;
    }

    public static bool UninstallDriver()
    {
        return InputInterceptor.UninstallDriver();
    }

    public bool Start()
    {
        if (!IsDriverInstalled)
        {
            Log("Start() failed: Driver is not installed.");
            return false;
        }

        if (_isRunning) return true;

        if (!InputInterceptor.Initialize())
        {
            Log("Start() failed: InputInterceptor.Initialize() returned false.");
            return false;
        }

        _context = CreateContextSafe();
        if (_context == IntPtr.Zero)
        {
            Log("Start() failed: Could not create context.");
            return false;
        }

        // Set filter to intercept all keyboard events
        InputInterceptor.SetFilter(_context, InputInterceptor.IsKeyboard, KeyboardFilter.All);

        // Auto-detect laptop keyboard if not already set
        AutoDetectLaptopDevice();

        _isRunning = true;
        _workerThread = new Thread(WorkerLoop)
        {
            IsBackground = true,
            Name = "InterceptionWorker",
            Priority = ThreadPriority.Normal
        };
        _workerThread.Start();
        Log($"Worker thread started. LaptopDeviceId={_laptopDeviceId}, IsLaptopDisabled={_isLaptopDisabled}");

        return true;
    }

    public void Stop()
    {
        _isRunning = false;
        if (_context != IntPtr.Zero)
        {
            // Reset filter so no keys are held up
            try
            {
                InputInterceptor.SetFilter(_context, InputInterceptor.IsKeyboard, KeyboardFilter.None);
                DestroyContextSafe(_context);
            }
            catch (Exception ex)
            {
                Log($"Stop() error: {ex.Message}");
            }
            _context = IntPtr.Zero;
        }

        if (_workerThread != null && _workerThread.IsAlive)
        {
            _workerThread.Join(500);
            _workerThread = null;
        }

        try
        {
            InputInterceptor.Dispose();
        }
        catch { }

        Log("Worker thread stopped and resources disposed.");
    }

    public void SetLaptopKeyboardDisabled(bool disabled)
    {
        _isLaptopDisabled = disabled;
        Log($"SetLaptopKeyboardDisabled({disabled}) applied for Device #{_laptopDeviceId}");
        StatusChanged?.Invoke(_isLaptopDisabled);
    }

    public void ToggleLaptopKeyboard()
    {
        SetLaptopKeyboardDisabled(!_isLaptopDisabled);
    }

    public void SetLaptopDeviceId(int deviceId)
    {
        _laptopDeviceId = deviceId;
        _config.LaptopDeviceId = deviceId;
        SaveConfig();
        Log($"LaptopDeviceId explicitly updated to: {deviceId}");
    }

    public void StartIdentifyingDevice()
    {
        _isIdentifying = true;
        Log("Keyboard identification mode started.");
    }

    public void CancelIdentifyingDevice()
    {
        _isIdentifying = false;
        Log("Keyboard identification mode cancelled.");
    }

    public List<KeyboardDeviceItem> GetKeyboardDevices()
    {
        var result = new List<KeyboardDeviceItem>();
        try
        {
            IntPtr ctx = _context;
            bool tempCreated = false;
            if (ctx == IntPtr.Zero)
            {
                ctx = CreateContextSafe();
                tempCreated = true;
            }

            if (ctx != IntPtr.Zero)
            {
                var list = InputInterceptor.GetDeviceList(ctx, InputInterceptor.IsKeyboard);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        string name = item.CompositeName ?? string.Join("; ", item.Names ?? new List<string>());
                        if (string.IsNullOrWhiteSpace(name)) name = $"Bàn phím #{item.Device}";

                        bool isLikelyLaptop = name.Contains("ACPI", StringComparison.OrdinalIgnoreCase) ||
                                              name.Contains("KBC", StringComparison.OrdinalIgnoreCase) ||
                                              name.Contains("PS/2", StringComparison.OrdinalIgnoreCase) ||
                                              name.Contains("PNP0303", StringComparison.OrdinalIgnoreCase);

                        result.Add(new KeyboardDeviceItem
                        {
                            DeviceId = item.Device,
                            Name = name,
                            IsLikelyLaptop = isLikelyLaptop
                        });
                    }
                }

                if (tempCreated)
                {
                    DestroyContextSafe(ctx);
                }
            }
        }
        catch (Exception ex)
        {
            Log($"GetKeyboardDevices error: {ex.Message}");
        }

        // If list is empty or couldn't get names, list standard 1..10
        if (result.Count == 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                result.Add(new KeyboardDeviceItem
                {
                    DeviceId = i,
                    Name = $"Bàn phím #{i}",
                    IsLikelyLaptop = (i == 1)
                });
            }
        }

        return result;
    }

    private void AutoDetectLaptopDevice()
    {
        if (_laptopDeviceId > 0) return; // Already configured

        var devices = GetKeyboardDevices();
        var laptop = devices.FirstOrDefault(d => d.IsLikelyLaptop);
        if (laptop != null)
        {
            _laptopDeviceId = laptop.DeviceId;
            Log($"AutoDetected laptop keyboard: Device #{_laptopDeviceId} ({laptop.Name})");
        }
        else if (devices.Count > 0)
        {
            // Device 1 is typically PS/2 laptop keyboard
            _laptopDeviceId = devices[0].DeviceId;
            Log($"Defaulted laptop keyboard: Device #{_laptopDeviceId} ({devices[0].Name})");
        }
        else
        {
            _laptopDeviceId = 1;
            Log("Defaulted laptop keyboard to Device #1");
        }

        _config.LaptopDeviceId = _laptopDeviceId;
        SaveConfig();
    }

    private void WorkerLoop()
    {
        while (_isRunning && _context != IntPtr.Zero)
        {
            int device = InputInterceptor.Wait(_context);
            if (device == 0 || !_isRunning) break;

            Stroke stroke = default;
            int received = InputInterceptor.Receive(_context, device, ref stroke, 1);

            if (received > 0)
            {
                // Identification mode
                if (_isIdentifying)
                {
                    _isIdentifying = false;
                    _laptopDeviceId = device;
                    _config.LaptopDeviceId = device;
                    SaveConfig();

                    string devName = GetDeviceName(device);
                    Log($"Key pressed during identification: Device #{device} ({devName})");
                    DeviceIdentified?.Invoke(device, devName);
                }

                // If this is the laptop keyboard and it is marked disabled: DROP IT!
                if (_isLaptopDisabled && device == _laptopDeviceId)
                {
                    // Swallowing the keystroke!
                    continue;
                }

                // Pass the keystroke through immediately
                InputInterceptor.Send(_context, device, ref stroke, 1);
            }
        }
    }

    private string GetDeviceName(int device)
    {
        try
        {
            if (_context != IntPtr.Zero)
            {
                var list = InputInterceptor.GetDeviceList(_context, InputInterceptor.IsKeyboard);
                var found = list?.FirstOrDefault(d => d.Device == device);
                if (found != null)
                {
                    return found.CompositeName ?? string.Join("; ", found.Names ?? new List<string>());
                }
            }
        }
        catch { }
        return $"Bàn phím #{device}";
    }

    private IntPtr CreateContextSafe()
    {
        // Try standard context first
        try
        {
            IntPtr ctx = InputInterceptor.CreateContext();
            if (ctx != IntPtr.Zero)
            {
                _isCustomContext = false;
                Log("Created standard Interception context successfully.");
                return ctx;
            }
        }
        catch { }

        // Fallback: Create custom context for available keyboard devices without mouse dependency
        Log("Standard CreateContext failed (mouse filter removed). Creating keyboard-only custom context...");
        IntPtr customCtx = CreateKeyboardOnlyContext();
        if (customCtx != IntPtr.Zero)
        {
            _isCustomContext = true;
            Log("Custom keyboard-only context created successfully.");
        }
        else
        {
            Log("Failed to create custom keyboard-only context.");
        }
        return customCtx;
    }

    private static IntPtr CreateKeyboardOnlyContext()
    {
        int itemSize = Marshal.SizeOf<InterceptionDeviceItem>();
        IntPtr context = Marshal.AllocHGlobal(itemSize * 20);

        // Zero out memory
        byte[] zero = new byte[itemSize * 20];
        Marshal.Copy(zero, 0, context, zero.Length);

        int successfulKeyboards = 0;

        for (int i = 0; i < 20; i++)
        {
            string devName = $@"\\.\interception{i:D2}";
            IntPtr hFile = CreateFile(devName, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (hFile != IntPtr.Zero && hFile != new IntPtr(-1))
            {
                IntPtr hEvent = CreateEvent(IntPtr.Zero, true, false, null);
                if (hEvent != IntPtr.Zero)
                {
                    IntPtr[] zeroPadded = new IntPtr[] { hEvent, IntPtr.Zero };
                    int structSize = IntPtr.Size * 2;
                    IntPtr pZeroPadded = Marshal.AllocHGlobal(structSize);
                    Marshal.Copy(zeroPadded, 0, pZeroPadded, 2);

                    bool ioctlSuccess = DeviceIoControl(hFile, IOCTL_SET_EVENT, pZeroPadded, (uint)structSize, IntPtr.Zero, 0, out uint bytesRet, IntPtr.Zero);
                    Marshal.FreeHGlobal(pZeroPadded);

                    if (ioctlSuccess)
                    {
                        InterceptionDeviceItem item = new InterceptionDeviceItem
                        {
                            Handle = hFile,
                            Unempty = hEvent
                        };
                        IntPtr offset = IntPtr.Add(context, i * itemSize);
                        Marshal.StructureToPtr(item, offset, false);
                        successfulKeyboards++;
                        continue;
                    }
                    CloseHandle(hEvent);
                }
                CloseHandle(hFile);
            }
        }

        if (successfulKeyboards == 0)
        {
            Marshal.FreeHGlobal(context);
            return IntPtr.Zero;
        }

        return context;
    }

    private void DestroyContextSafe(IntPtr context)
    {
        if (context == IntPtr.Zero) return;

        if (_isCustomContext)
        {
            int itemSize = Marshal.SizeOf<InterceptionDeviceItem>();
            for (int i = 0; i < 20; i++)
            {
                IntPtr offset = IntPtr.Add(context, i * itemSize);
                InterceptionDeviceItem item = Marshal.PtrToStructure<InterceptionDeviceItem>(offset);
                if (item.Handle != IntPtr.Zero && item.Handle != new IntPtr(-1))
                {
                    CloseHandle(item.Handle);
                }
                if (item.Unempty != IntPtr.Zero)
                {
                    CloseHandle(item.Unempty);
                }
            }
            Marshal.FreeHGlobal(context);
        }
        else
        {
            try
            {
                InputInterceptor.DestroyContext(context);
            }
            catch { }
        }
    }

    private void LoadConfig()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                string json = File.ReadAllText(ConfigPath);
                var cfg = JsonSerializer.Deserialize<ConfigData>(json);
                if (cfg != null)
                {
                    _config = cfg;
                    _laptopDeviceId = _config.LaptopDeviceId;
                    _isLaptopDisabled = _config.IsDisabledOnStartup;
                    Log($"Loaded config: LaptopDeviceId={_laptopDeviceId}, IsDisabledOnStartup={_config.IsDisabledOnStartup}");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"LoadConfig error: {ex.Message}");
        }
    }

    public void SaveConfig()
    {
        try
        {
            string? dir = Path.GetDirectoryName(ConfigPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _config.LaptopDeviceId = _laptopDeviceId;
            string json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
        catch (Exception ex)
        {
            Log($"SaveConfig error: {ex.Message}");
        }
    }

    public static void Log(string message)
    {
        try
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LaptopKeyboardDisabler",
                "app.log"
            );
            string? dir = Path.GetDirectoryName(logPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}\r\n");
        }
        catch { }
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr CreateEvent(
        IntPtr lpEventAttributes,
        bool bManualReset,
        bool bInitialState,
        string? lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeviceIoControl(
        IntPtr hDevice,
        uint dwIoControlCode,
        IntPtr lpInBuffer,
        uint nInBufferSize,
        IntPtr lpOutBuffer,
        uint nOutBufferSize,
        out uint lpBytesReturned,
        IntPtr lpOverlapped);
}
