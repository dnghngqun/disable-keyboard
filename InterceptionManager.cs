using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private IntPtr _context = IntPtr.Zero;
    private Thread? _workerThread;
    private volatile bool _isRunning = false;
    private volatile bool _isLaptopDisabled = false;
    private volatile bool _isIdentifying = false;

    private int _laptopDeviceId = -1;
    private ConfigData _config = new();

    public event Action<bool>? StatusChanged;
    public event Action<int, string>? DeviceIdentified;

    public bool IsDriverInstalled => InputInterceptor.CheckDriverInstalled();
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
        return InputInterceptor.InstallDriver();
    }

    public static bool UninstallDriver()
    {
        return InputInterceptor.UninstallDriver();
    }

    public bool Start()
    {
        if (!IsDriverInstalled)
        {
            return false;
        }

        if (_isRunning) return true;

        if (!InputInterceptor.Initialize())
        {
            return false;
        }

        _context = InputInterceptor.CreateContext();
        if (_context == IntPtr.Zero)
        {
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
            Priority = ThreadPriority.Highest
        };
        _workerThread.Start();

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
                InputInterceptor.DestroyContext(_context);
            }
            catch { }
            _context = IntPtr.Zero;
        }

        if (_workerThread != null && _workerThread.IsAlive)
        {
            _workerThread.Join(500);
            _workerThread = null;
        }

        InputInterceptor.Dispose();
    }

    public void SetLaptopKeyboardDisabled(bool disabled)
    {
        _isLaptopDisabled = disabled;
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
    }

    public void StartIdentifyingDevice()
    {
        _isIdentifying = true;
    }

    public void CancelIdentifyingDevice()
    {
        _isIdentifying = false;
    }

    public List<KeyboardDeviceItem> GetKeyboardDevices()
    {
        var result = new List<KeyboardDeviceItem>();
        try
        {
            var list = InputInterceptor.GetDeviceList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (InputInterceptor.IsKeyboard(item.Device))
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
            }
        }
        catch { }

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
        }
        else if (devices.Count > 0)
        {
            // By Interception convention, Device 1 is typically PS/2 keyboard
            _laptopDeviceId = devices[0].DeviceId;
        }
        else
        {
            _laptopDeviceId = 1;
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
            var list = InputInterceptor.GetDeviceList();
            var found = list?.FirstOrDefault(d => d.Device == device);
            if (found != null)
            {
                return found.CompositeName ?? string.Join("; ", found.Names ?? new List<string>());
            }
        }
        catch { }
        return $"Bàn phím #{device}";
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
                }
            }
        }
        catch { }
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
        catch { }
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}
