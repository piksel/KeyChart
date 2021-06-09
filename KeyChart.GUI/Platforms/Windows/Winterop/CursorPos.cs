using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeyChart.GUI.Platforms.Windows.Winterop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorPos
    {
        public int X;
        public int Y;
        
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out CursorPos lpPoint);

        public static CursorPos Current
            => GetCursorPos(out CursorPos cursorPos) ? cursorPos : new CursorPos();

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}