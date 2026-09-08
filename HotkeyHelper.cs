using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LaptopKeyboardDisabler;

public class HotkeyHelper : IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_NOREPEAT = 0x4000;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private readonly IntPtr _handle;
    private readonly int _hotkeyId;
    private bool _registered;

    public event Action? HotkeyPressed;

    public HotkeyHelper(IntPtr windowHandle, int id = 9001)
    {
        _handle = windowHandle;
        _hotkeyId = id;
    }

    public bool Register(Keys key, bool ctrl = true, bool alt = true)
    {
        if (_registered)
        {
            Unregister();
        }

        uint modifiers = MOD_NOREPEAT;
        if (ctrl) modifiers |= MOD_CONTROL;
        if (alt) modifiers |= MOD_ALT;

        _registered = RegisterHotKey(_handle, _hotkeyId, modifiers, (uint)key);
        return _registered;
    }

    public void Unregister()
    {
        if (_registered)
        {
            UnregisterHotKey(_handle, _hotkeyId);
            _registered = false;
        }
    }

    public bool ProcessMessage(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == _hotkeyId)
        {
            HotkeyPressed?.Invoke();
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        Unregister();
        GC.SuppressFinalize(this);
    }
}
