using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using KeyChart.Services;
using A = Avalonia;

namespace KeyChart.Avalonia.Util
{
    public class CustomWin32WindowImpl: A.Win32.WindowImpl
    {
        const int Pause = 0x13;
        
        protected override IntPtr CreateWindowOverride(ushort atom)
        {
            // var handle = base.CreateWindowOverride(atom);

            var handle = Winterop.CreateWindowEx(
                (int)WindowStyles.WS_EX_LAYERED,
                //0, //(int)WindowStyles.WS_EX_NOREDIRECTIONBITMAP,
                atom,
                null,
                (uint)WindowStyles.WS_POPUPWINDOW  | (uint)WindowStyles.WS_CLIPCHILDREN,
                Winterop.CW_USEDEFAULT,
                Winterop.CW_USEDEFAULT,
                Winterop.CW_USEDEFAULT,
                Winterop.CW_USEDEFAULT,
                IntPtr.Zero,
                IntPtr.Zero, 
                IntPtr.Zero, 
                IntPtr.Zero
            );

            var action = HotkeyService.HotkeyAction.ShowOverlay;
            var modifiers = KeyModifiers.None;
            var vlc = Pause;
            Winterop.RegisterHotKey(handle, (int) action, (int) modifiers, (int) vlc);

            
            return handle;
        }

        public bool ShowingOverview { get; set; }

        public byte Alpha { get; set; } = 255;
        
        protected override IntPtr AppWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            
            switch (msg)
            {
                case Winterop.WM_HOTKEY:
                    var action = (HotkeyService.HotkeyAction) wParam.ToInt32();
                    if (!ShowingOverview)
                    {
                        if (Winterop.IsWindowVisible(hWnd))
                        {
                            this.Hide();
                            //this.Show(true);
                        }
                        this.SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints.NoChrome);

                        //this.WindowState = WindowState.Normal;
                        // this.WindowState = WindowState.FullScreen;
                        this.Position = A.PixelPoint.Origin;
                        // this.WindowState = WindowState.FullScreen;
                        // Winterop.SetLayeredWindowAttributes(Hwnd, 0, Alpha, 2);
                        // Winterop.AnimateWindow(hWnd, 500, Winterop.AW_CENTER | Winterop.AW_ACTIVATE);
                        this.Show(true);
                        //this.WindowState = WindowState.FullScreen;
                        this.WindowState = WindowState.Maximized;
                        //this.ShowTaskbarIcon(false);

                        //this.Show(true);
                        this.SetTopmost(true);
                        FadeIn();
                        //ShowingOverview = !ShowingOverview;

                    }
                    else
                    {
                        
                        // this.SetTopmost(false);
                        // //this.WindowState = WindowState.Minimized;
                        // Winterop.AnimateWindow(hWnd, 500, Winterop.AW_HOR_POSITIVE | Winterop.AW_VER_NEGATIVE | Winterop.AW_HIDE);
                        // this.SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints.SystemChrome);
                        // //this.Hide();
                        FadeOut();
                    }

                    

                    Debug.WriteLine($"GOT HOTKEY!!! {0:G}", action);
                    break;

                case Winterop.WM_DESTROY: //unregister all hot keys
                    //foreach (var action in _registeredActions)
                    //{
                    Winterop.UnregisterHotKey(hWnd, (int) HotkeyService.HotkeyAction.ShowOverlay);
                    //}

                    break;
                default:
                    // Debug.WriteLine("WndProc: {0:x}, {1:x}, {2:x}", msg, wParam, lParam);
                    break;
            }

            return base.AppWndProc(hWnd, msg, wParam, lParam);
        }

        private void FadeOut()
        {
            DispatcherTimer.Run(() =>
            {
                int newAlpha = Alpha - 16;
                var done = newAlpha <= 0;
                Alpha =  (byte)(done ? 0 : newAlpha);

                Winterop.SetLayeredWindowAttributes(Hwnd, 0, Alpha, 2);
                if (done)
                {
                    ShowingOverview = !ShowingOverview;
                    SetTopmost(false);
                    Hide();
                }

                return !done;
            }, TimeSpan.FromMilliseconds(10), DispatcherPriority.Render);
        }
        
        private void FadeIn()
        {
            DispatcherTimer.Run(() =>
            {
                int newAlpha = Alpha + 16;
                var done = newAlpha >= 255;
                Alpha =  (byte)(done ? 255 : newAlpha);

                Winterop.SetLayeredWindowAttributes(Hwnd, 0, Alpha, 2);
                if (done)
                {
                    ShowingOverview = !ShowingOverview;
                    // SetTopmost(false);
                }

                return !done;
            }, TimeSpan.FromMilliseconds(10), DispatcherPriority.Render);
        }
    }

    public class CustomWin32EmbedWindowImpl : CustomWin32WindowImpl
    {
        // protected override IntPtr CreateWindowOverride(ushort atom)
        // {
        //     var hWnd = Winterop.CreateWindowEx(
        //         0,
        //         atom,
        //         null,
        //         (int)WindowStyles.WS_CHILD,
        //         0,
        //         0,
        //         640,
        //         480,
        //         this.OffscreenParentWindow.Handle,
        //         IntPtr.Zero,
        //         IntPtr.Zero,
        //         IntPtr.Zero);
        //     return hWnd;
        // }
    }

    public class CustomWin32WindowingPlatform : IWindowingPlatform
    {
        public IWindowImpl CreateWindow()
        {
            return new CustomWin32WindowImpl();
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            Debug.Print("Call to broken CreateEmbeddableWindow()!");
            var embedded = new CustomWin32EmbedWindowImpl();
            embedded.Show(true);
            return embedded;
        }
    }
    
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

        [DllImport("user32.dll")]
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
    
    [Flags]
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