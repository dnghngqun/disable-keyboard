using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LaptopKeyboardDisabler;

internal static class Program
{
    private static Mutex? _mutex;

    [STAThread]
    static void Main(string[] args)
    {
        // 1. Immediately fix Mouse UpperFilters to ensure mouse / touchpad / bluetooth mouse never get blocked
        FixMouseUpperFilters();

        if (args.Any(a => a.Equals("--restore-defaults", StringComparison.OrdinalIgnoreCase)))
        {
            RestoreWindowsDefaults();
            return;
        }

        const string appGuid = "Global\\LaptopKeyboardDisabler_8F813D1A-6C91-44B4-884A-98C02B911762";
        _mutex = new Mutex(true, appGuid, out bool createdNew);

        if (!createdNew)
        {
            MessageBox.Show("Ứng dụng Laptop Keyboard Disabler đang chạy trong khay hệ thống (System Tray). Vui lòng kiểm tra góc phải thanh Taskbar.", 
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ApplicationConfiguration.Initialize();

        bool startMinimized = args.Any(a => a.Equals("--tray", StringComparison.OrdinalIgnoreCase) || 
                                           a.Equals("-tray", StringComparison.OrdinalIgnoreCase) ||
                                           a.Equals("--minimized", StringComparison.OrdinalIgnoreCase));

        Application.Run(new MainForm(startMinimized));

        _mutex.ReleaseMutex();
    }

    public static void FixMouseUpperFilters()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e96f-e325-11ce-bfc1-08002be10318}", true);
            if (key != null)
            {
                object? val = key.GetValue("UpperFilters");
                if (val is string[] filters)
                {
                    // Remove "mouse" filter from Interception
                    var cleanFilters = filters.Where(f => !f.Equals("mouse", StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (cleanFilters.Length == 0) cleanFilters = new[] { "mouclass" };

                    if (!filters.SequenceEqual(cleanFilters))
                    {
                        key.SetValue("UpperFilters", cleanFilters, RegistryValueKind.MultiString);
                        File.AppendAllText(@"d:\fix_mouse.log", $"[{DateTime.Now}] Mouse UpperFilters cleaned to: {string.Join(", ", cleanFilters)}\n");

                        // Restart mouse devices so mouse / touchpad comes back immediately without reboot
                        try
                        {
                            var psi = new ProcessStartInfo
                            {
                                FileName = "pnputil.exe",
                                Arguments = "/restart-device /class Mouse",
                                CreateNoWindow = true,
                                UseShellExecute = false
                            };
                            Process.Start(psi)?.WaitForExit(5000);
                        }
                        catch { }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(@"d:\fix_mouse.log", $"[{DateTime.Now}] Fix error: {ex.Message}\n");
        }
    }

    public static void RestoreWindowsDefaults()
    {
        try
        {
            // Restore Mouse UpperFilters strictly to mouclass
            using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e96f-e325-11ce-bfc1-08002be10318}", true))
            {
                key?.SetValue("UpperFilters", new[] { "mouclass" }, RegistryValueKind.MultiString);
            }

            // Restore Keyboard UpperFilters strictly to kbdclass
            using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e96b-e325-11ce-bfc1-08002be10318}", true))
            {
                key?.SetValue("UpperFilters", new[] { "kbdclass" }, RegistryValueKind.MultiString);
            }

            File.AppendAllText(@"d:\fix_mouse.log", $"[{DateTime.Now}] Completely restored Mouse and Keyboard defaults!\n");

            // Restart devices
            try
            {
                Process.Start(new ProcessStartInfo("pnputil.exe", "/restart-device /class Mouse") { CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(5000);
                Process.Start(new ProcessStartInfo("pnputil.exe", "/restart-device /class Keyboard") { CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(5000);
            }
            catch { }
        }
        catch (Exception ex)
        {
            File.AppendAllText(@"d:\fix_mouse.log", $"[{DateTime.Now}] Restore error: {ex.Message}\n");
        }
    }
}
