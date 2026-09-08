using System;
using System.Diagnostics;
using System.IO;

namespace LaptopKeyboardDisabler;

public static class StartupManager
{
    private const string TaskName = "LaptopKeyboardDisabler";

    public static bool IsStartupEnabled()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $"/Query /TN \"{TaskName}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return false;
            process.WaitForExit(3000);
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public static (bool Success, string Message) SetStartup(bool enable)
    {
        try
        {
            if (enable)
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName 
                                 ?? Environment.ProcessPath 
                                 ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LaptopKeyboardDisabler.exe");

                // Use --tray flag so it boots silently to tray
                string runCommand = $"\\\"{exePath}\\\" --tray";
                string args = $"/Create /TN \"{TaskName}\" /TR \"{runCommand}\" /SC ONLOGON /RL HIGHEST /F";

                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null) return (false, "Không thể khởi động schtasks.");
                process.WaitForExit(5000);

                string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                if (process.ExitCode == 0)
                {
                    return (true, "Đã bật khởi động cùng Windows (chạy ngầm dưới khay, không hiện UAC).");
                }
                else
                {
                    return (false, "Lỗi khi tạo Task: " + output.Trim());
                }
            }
            else
            {
                string args = $"/Delete /TN \"{TaskName}\" /F";

                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null) return (false, "Không thể khởi động schtasks.");
                process.WaitForExit(5000);

                return (true, "Đã tắt khởi động cùng Windows.");
            }
        }
        catch (Exception ex)
        {
            return (false, "Ngoại lệ: " + ex.Message);
        }
    }
}
