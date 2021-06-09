using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia.Input;
using Microsoft.Win32.SafeHandles;

namespace KeyChart.GUI.Util
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Winterop
    {
        public const int CW_USEDEFAULT = unchecked((int)0x80000000);
        
        public const int WM_HOTKEY = 0x0312;
        public const int WM_DESTROY = 0x0002;

        public const int AW_ACTIVATE = 0x00020000;
        public const int AW_BLEND = 0x00080000;
        public const int AW_CENTER = 0x00000010;
        public const int AW_HIDE = 0x00010000;
        public const int AW_HOR_POSITIVE = 0x00000001;
        public const int AW_HOR_NEGATIVE = 0x00000002;
        public const int AW_VER_POSITIVE = 0x00000004;
        public const int AW_VER_NEGATIVE = 0x00000008;
        public const int AW_SLIDE = 0x00040000;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);
        
        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(
            IntPtr     hWnd,
            uint crKey,
            byte     bAlpha,
            uint    dwFlags
        );
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            int dwExStyle,
            uint lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);
        
        // CreateFile
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            [MarshalAs(UnmanagedType.LPTStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        public static int VirtualKeyFromKey(Key key)
        {
            SVirtualKeyFromKey.TryGetValue(key, out var result);

            return result;
        }
        
        private static readonly Dictionary<Key, int> SVirtualKeyFromKey = new() 
        {
            { Key.None, 0 },
            { Key.Cancel, 3 },
            { Key.Back, 8 },
            { Key.Tab, 9 },
            { Key.LineFeed, 0 },
            { Key.Clear, 12 },
            { Key.Return, 13 },
            { Key.Pause, 19 },
            { Key.Capital, 20 },
            { Key.KanaMode, 21 },
            { Key.JunjaMode, 23 },
            { Key.FinalMode, 24 },
            { Key.HanjaMode, 25 },
            { Key.Escape, 27 },
            { Key.ImeConvert, 28 },
            { Key.ImeNonConvert, 29 },
            { Key.ImeAccept, 30 },
            { Key.ImeModeChange, 31 },
            { Key.Space, 32 },
            { Key.PageUp, 33 },
            { Key.Next, 34 },
            { Key.End, 35 },
            { Key.Home, 36 },
            { Key.Left, 37 },
            { Key.Up, 38 },
            { Key.Right, 39 },
            { Key.Down, 40 },
            { Key.Select, 41 },
            { Key.Print, 42 },
            { Key.Execute, 43 },
            { Key.Snapshot, 44 },
            { Key.Insert, 45 },
            { Key.Delete, 46 },
            { Key.Help, 47 },
            { Key.D0, 48 },
            { Key.D1, 49 },
            { Key.D2, 50 },
            { Key.D3, 51 },
            { Key.D4, 52 },
            { Key.D5, 53 },
            { Key.D6, 54 },
            { Key.D7, 55 },
            { Key.D8, 56 },
            { Key.D9, 57 },
            { Key.A, 65 },
            { Key.B, 66 },
            { Key.C, 67 },
            { Key.D, 68 },
            { Key.E, 69 },
            { Key.F, 70 },
            { Key.G, 71 },
            { Key.H, 72 },
            { Key.I, 73 },
            { Key.J, 74 },
            { Key.K, 75 },
            { Key.L, 76 },
            { Key.M, 77 },
            { Key.N, 78 },
            { Key.O, 79 },
            { Key.P, 80 },
            { Key.Q, 81 },
            { Key.R, 82 },
            { Key.S, 83 },
            { Key.T, 84 },
            { Key.U, 85 },
            { Key.V, 86 },
            { Key.W, 87 },
            { Key.X, 88 },
            { Key.Y, 89 },
            { Key.Z, 90 },
            { Key.LWin, 91 },
            { Key.RWin, 92 },
            { Key.Apps, 93 },
            { Key.Sleep, 95 },
            { Key.NumPad0, 96 },
            { Key.NumPad1, 97 },
            { Key.NumPad2, 98 },
            { Key.NumPad3, 99 },
            { Key.NumPad4, 100 },
            { Key.NumPad5, 101 },
            { Key.NumPad6, 102 },
            { Key.NumPad7, 103 },
            { Key.NumPad8, 104 },
            { Key.NumPad9, 105 },
            { Key.Multiply, 106 },
            { Key.Add, 107 },
            { Key.Separator, 108 },
            { Key.Subtract, 109 },
            { Key.Decimal, 110 },
            { Key.Divide, 111 },
            { Key.F1, 112 },
            { Key.F2, 113 },
            { Key.F3, 114 },
            { Key.F4, 115 },
            { Key.F5, 116 },
            { Key.F6, 117 },
            { Key.F7, 118 },
            { Key.F8, 119 },
            { Key.F9, 120 },
            { Key.F10, 121 },
            { Key.F11, 122 },
            { Key.F12, 123 },
            { Key.F13, 124 },
            { Key.F14, 125 },
            { Key.F15, 126 },
            { Key.F16, 127 },
            { Key.F17, 128 },
            { Key.F18, 129 },
            { Key.F19, 130 },
            { Key.F20, 131 },
            { Key.F21, 132 },
            { Key.F22, 133 },
            { Key.F23, 134 },
            { Key.F24, 135 },
            { Key.NumLock, 144 },
            { Key.Scroll, 145 },
            { Key.LeftShift, 160 },
            { Key.RightShift, 161 },
            { Key.LeftCtrl, 162 },
            { Key.RightCtrl, 163 },
            { Key.LeftAlt, 164 },
            { Key.RightAlt, 165 },
            { Key.BrowserBack, 166 },
            { Key.BrowserForward, 167 },
            { Key.BrowserRefresh, 168 },
            { Key.BrowserStop, 169 },
            { Key.BrowserSearch, 170 },
            { Key.BrowserFavorites, 171 },
            { Key.BrowserHome, 172 },
            { Key.VolumeMute, 173 },
            { Key.VolumeDown, 174 },
            { Key.VolumeUp, 175 },
            { Key.MediaNextTrack, 176 },
            { Key.MediaPreviousTrack, 177 },
            { Key.MediaStop, 178 },
            { Key.MediaPlayPause, 179 },
            { Key.LaunchMail, 180 },
            { Key.SelectMedia, 181 },
            { Key.LaunchApplication1, 182 },
            { Key.LaunchApplication2, 183 },
            { Key.Oem1, 186 },
            { Key.OemPlus, 187 },
            { Key.OemComma, 188 },
            { Key.OemMinus, 189 },
            { Key.OemPeriod, 190 },
            { Key.OemQuestion, 191 },
            { Key.Oem3, 192 },
            { Key.AbntC1, 193 },
            { Key.AbntC2, 194 },
            { Key.OemOpenBrackets, 219 },
            { Key.Oem5, 220 },
            { Key.Oem6, 221 },
            { Key.OemQuotes, 222 },
            { Key.Oem8, 223 },
            { Key.OemBackslash, 226 },
            { Key.ImeProcessed, 229 },
            { Key.System, 0 },
            { Key.OemAttn, 240 },
            { Key.OemFinish, 241 },
            { Key.OemCopy, 242 },
            { Key.DbeSbcsChar, 243 },
            { Key.OemEnlw, 244 },
            { Key.OemBackTab, 245 },
            { Key.DbeNoRoman, 246 },
            { Key.DbeEnterWordRegisterMode, 247 },
            { Key.DbeEnterImeConfigureMode, 248 },
            { Key.EraseEof, 249 },
            { Key.Play, 250 },
            { Key.DbeNoCodeInput, 251 },
            { Key.NoName, 252 },
            { Key.Pa1, 253 },
            { Key.OemClear, 254 },
            { Key.DeadCharProcessed, 0 },
        };
        
    
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum WindowStyles : uint
        {
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xc00000,
            WS_CHILD = 0x40000000,
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_GROUP = 0x20000,
            WS_HSCROLL = 0x100000,
            WS_MAXIMIZE = 0x1000000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x20000,
            WS_OVERLAPPED = 0x0,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000u,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEFRAME = 0x40000,
            WS_SYSMENU = 0x80000,
            WS_TABSTOP = 0x10000,
            WS_THICKFRAME = 0x40000,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x200000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000
        }
    }
    
}