using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Threading;
using KeyChart.GUI.Platforms.Windows.Winterop;
using KeyChart.Services;

namespace KeyChart.GUI.Util
{
    public class HotkeyWindowImplWin32 : Avalonia.Win32.WindowImpl
    {
        public event EventHandler<HotkeyService.HotkeyAction>? HotkeyPressed;
        
        List<HotkeyService.HotkeyAction> HotkeyActions { get; set; } = new();
        
        protected override IntPtr CreateWindowOverride(ushort atom)
        {
            var handle = base.CreateWindowOverride(atom);

            //RegisterHotKeys(handle);

            return handle;
        }

        public void RegisterHotkey(HotkeyService.HotkeyAction action, KeyModifiers modifiers, Key key)
        {
            var vlc = Winterop.VirtualKeyFromKey(key);
            if (!Winterop.RegisterHotKey(Hwnd, (int) action, (int) modifiers, (int) vlc))
            {
                var hres = Marshal.GetHRForLastWin32Error();
                var ex = Marshal.GetExceptionForHR(hres);
                Debug.WriteLine("Failed to register Hotkey. Handle is 0x{0:x}, error is: 0x{1:x}, hres: 0x{2:x}, exception: {3}", Hwnd, Marshal.GetLastWin32Error(), hres, ex);
            }
            HotkeyActions.Add(action);
        }

        public event EventHandler<CustomWndProcEventArgs>? CustomWndProc;

        protected override IntPtr AppWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            var message = (WindowMessage) msg;
            switch (message)
            {
                case WindowMessage.WM_HOTKEY:
                    var action = (HotkeyService.HotkeyAction) wParam.ToInt32();
                    HotkeyPressed?.Invoke(this, action);
                    Debug.WriteLine("GOT HOTKEY!!! {0:G}", action);
                    break;

                case WindowMessage.WM_DESTROY: //unregister all hot keys
                    foreach (var regAction in HotkeyActions)
                    {
                        Winterop.UnregisterHotKey(hWnd, (int) regAction);
                    }
                    break;
            }
            
            CustomWndProc?.Invoke(this, new CustomWndProcEventArgs(hWnd, message, wParam, lParam));

            return base.AppWndProc(hWnd, msg, wParam, lParam);
        }
    }

    public class CustomWndProcEventArgs: EventArgs
    {
        public CustomWndProcEventArgs(IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam)
        {
            Handle = handle;
            Message = message;
            LParam = lParam;
            WParam = wParam;
        }
        public IntPtr Handle { get; }
        public WindowMessage Message { get; }
        public IntPtr WParam { get; }
        public IntPtr LParam { get; }
    }


    public class OverlayWindowImplWin32: Avalonia.Win32.WindowImpl
    {
        const int Pause = 0x13;
        
        protected override IntPtr CreateWindowOverride(ushort atom)
        {
            // var handle = base.CreateWindowOverride(atom);

            var handle = Winterop.CreateWindowEx(
                (int)Winterop.WindowStyles.WS_EX_LAYERED,
                //0, //(int)WindowStyles.WS_EX_NOREDIRECTIONBITMAP,
                atom,
                null,
                (uint)Winterop.WindowStyles.WS_POPUPWINDOW  | (uint)Winterop.WindowStyles.WS_CLIPCHILDREN,
                Winterop.CW_USEDEFAULT,
                Winterop.CW_USEDEFAULT,
                Winterop.CW_USEDEFAULT,
                Winterop.CW_USEDEFAULT,
                IntPtr.Zero,
                IntPtr.Zero, 
                IntPtr.Zero, 
                IntPtr.Zero
            );
            
            return handle;
        }

        public bool ShowingOverview { get; set; }

        public byte Alpha { get; set; } = 255;

        public void ToggleOverview()
        {
            if (!ShowingOverview)
            {
                if (Winterop.IsWindowVisible(Hwnd))
                {
                    this.Hide();
                    //this.Show(true);
                }
                this.SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints.NoChrome);

                //this.WindowState = WindowState.Normal;
                // this.WindowState = WindowState.FullScreen;
                this.Position = Avalonia.PixelPoint.Origin;
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
    
}