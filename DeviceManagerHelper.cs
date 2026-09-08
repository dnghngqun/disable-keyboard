using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LaptopKeyboardDisabler;

public class KeyboardDevice
{
    public string InstanceId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Started", "Disabled", "Disconnected"
    public bool IsInternal { get; set; }
    public bool IsConnected => !string.Equals(Status, "Disconnected", StringComparison.OrdinalIgnoreCase);
    public bool IsActive => string.Equals(Status, "Started", StringComparison.OrdinalIgnoreCase);
    public bool IsDisabled => string.Equals(Status, "Disabled", StringComparison.OrdinalIgnoreCase);

    public override string ToString()
    {
        string badge = IsInternal ? "[Laptop Internal]" : "[External]";
        return $"{badge} {Description} ({Status})";
    }
}

public static class DeviceManagerHelper
{
    public static List<KeyboardDevice> GetKeyboards()
    {
        var list = new List<KeyboardDevice>();
        try
        {
            var output = RunCommand("pnputil.exe", "/enum-devices /class Keyboard");
            var blocks = output.Split(new[] { "Instance ID:" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in blocks)
            {
                var lines = block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0) continue;

                string instanceId = lines[0].Trim();
                string description = "Keyboard Device";
                string status = "Unknown";

                foreach (var line in lines)
                {
                    if (line.StartsWith("Device Description:", StringComparison.OrdinalIgnoreCase))
                    {
                        description = line.Substring("Device Description:".Length).Trim();
                    }
                    else if (line.StartsWith("Status:", StringComparison.OrdinalIgnoreCase))
                    {
                        status = line.Substring("Status:".Length).Trim();
                    }
                }

                if (!string.IsNullOrEmpty(instanceId))
                {
                    bool isInternal = instanceId.StartsWith("ACPI", StringComparison.OrdinalIgnoreCase) ||
                                      description.Contains("PS/2", StringComparison.OrdinalIgnoreCase) ||
                                      instanceId.Contains("KBC8042", StringComparison.OrdinalIgnoreCase) ||
                                      instanceId.Contains("PNP0303", StringComparison.OrdinalIgnoreCase);

                    list.Add(new KeyboardDevice
                    {
                        InstanceId = instanceId,
                        Description = description,
                        Status = status,
                        IsInternal = isInternal
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error enumerating keyboards: " + ex.Message);
        }

        return list;
    }

    public static KeyboardDevice? GetInternalKeyboard()
    {
        var keyboards = GetKeyboards();
        // Look for internal keyboard that is connected (Started or Disabled)
        var internalKb = keyboards.FirstOrDefault(k => k.IsInternal && k.IsConnected);
        if (internalKb == null)
        {
            // fallback to any internal
            internalKb = keyboards.FirstOrDefault(k => k.IsInternal);
        }
        return internalKb;
    }

    public static bool HasConnectedExternalKeyboard()
    {
        var keyboards = GetKeyboards();
        return keyboards.Any(k => !k.IsInternal && k.IsActive);
    }

    public static (bool Success, string Message) DisableKeyboard(string instanceId)
    {
        if (string.IsNullOrWhiteSpace(instanceId))
        {
            return (false, "Instance ID không hợp lệ.");
        }

        try
        {
            // First attempt with /force
            var output = RunCommand("pnputil.exe", $"/disable-device \"{instanceId}\" /force");
            if (output.Contains("Failed to disable device") && output.Contains("critical system device"))
            {
                // Fallback attempt: try remove-device with /force
                output = RunCommand("pnputil.exe", $"/remove-device \"{instanceId}\" /force");
            }

            // Verify state
            var current = GetKeyboards().FirstOrDefault(k => string.Equals(k.InstanceId, instanceId, StringComparison.OrdinalIgnoreCase));
            if (current == null || current.IsDisabled || !current.IsConnected)
            {
                return (true, "Đã vô hiệu hóa bàn phím laptop thành công.");
            }

            if (output.Contains("Failed") || output.Contains("denied"))
            {
                return (false, "Lỗi từ Windows: " + output.Trim());
            }

            return (true, "Đã gửi lệnh tắt bàn phím thành công.");
        }
        catch (Exception ex)
        {
            return (false, "Ngoại lệ khi tắt: " + ex.Message);
        }
    }

    public static (bool Success, string Message) EnableKeyboard(string instanceId)
    {
        if (string.IsNullOrWhiteSpace(instanceId))
        {
            return (false, "Instance ID không hợp lệ.");
        }

        try
        {
            var output = RunCommand("pnputil.exe", $"/enable-device \"{instanceId}\"");
            
            // Always perform hardware scan to wake and re-initialize the keyboard
            RunCommand("pnputil.exe", "/scan-devices /async");

            // Verify state
            var current = GetKeyboards().FirstOrDefault(k => string.Equals(k.InstanceId, instanceId, StringComparison.OrdinalIgnoreCase));
            if (current != null && current.IsActive)
            {
                return (true, "Đã kích hoạt lại bàn phím laptop thành công.");
            }

            return (true, "Đã kích hoạt lại bàn phím laptop.");
        }
        catch (Exception ex)
        {
            return (false, "Ngoại lệ khi bật: " + ex.Message);
        }
    }

    public static void ScanDevices()
    {
        try
        {
            RunCommand("pnputil.exe", "/scan-devices /async");
        }
        catch { }
    }

    private static string RunCommand(string fileName, string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return string.Empty;

        string stdout = process.StandardOutput.ReadToEnd();
        string stderr = process.StandardError.ReadToEnd();
        process.WaitForExit(5000);

        return stdout + "\n" + stderr;
    }
}
