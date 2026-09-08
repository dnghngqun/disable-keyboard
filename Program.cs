using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace LaptopKeyboardDisabler;

internal static class Program
{
    private static Mutex? _mutex;

    [STAThread]
    static void Main(string[] args)
    {
        const string appGuid = "Global\\LaptopKeyboardDisabler_8F813D1A-6C91-44B4-884A-98C02B911762";
        _mutex = new Mutex(true, appGuid, out bool createdNew);

        if (!createdNew)
        {
            // App is already running
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
}
