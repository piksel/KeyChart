using System;
using System.Runtime.InteropServices;

namespace KeyChart.HotkeyHelper
{
    public static class Winterop
    {
        public const int WM_HOTKEY = 0x0312;
        public const int WM_DESTROY = 0x0002;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        NoRepeat = 16384
    }
}