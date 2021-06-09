using System;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using KeyChart.GUI.Platforms.Windows.Winterop;
using KeyChart.GUI.Util;
using ReactiveUI;

namespace KeyChart.GUI.Platforms.Windows
{
    public class WindowsPlatformUtils: IPlatformUtils, IDisposable
    {
        public WindowsPlatformUtils()
        {
            _windowClass = RegisterWindowClass();
            _hwnd = CreateUtilWindow(_windowClass);
        }

        private string RegisterWindowClass()
        {
            var className = $"AppUtilWindow{string.Concat(Guid.NewGuid().ToByteArray().Select(b => $"{b:x2}"))}";
            var wc = new WindowClass
            {
                lpfnWndProc = OnWindowMessageReceived,
                lpszClassName = className,
            };
            
            WinApi.RegisterClass(ref wc);

            return className;
        }

        private static IntPtr CreateUtilWindow(string windowClass) 
            => WinApi.CreateWindowEx(
                dwExStyle: 0, 
                windowClass, 
                lpWindowName: string.Empty, 
                dwStyle: 0, 
                x: 0, 
                y: 0, 
                nWidth: 1, 
                nHeight: 1, 
                hWndParent: IntPtr.Zero, 
                hMenu: IntPtr.Zero,
                hInstance: IntPtr.Zero, 
                lpParam: IntPtr.Zero
            );

        private IntPtr OnWindowMessageReceived(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            var message = (WindowMessage) messageId;

            // Forward to listeners
            WindowMessageRecieved?.Invoke(this, new CustomWndProcEventArgs(hWnd, message, wParam, lParam));
            
            return WinApi.DefWindowProc(hWnd, messageId, wParam, lParam);
        }

        public event EventHandler<CustomWndProcEventArgs>? WindowMessageRecieved;
        private IntPtr _hwnd;
        private string?_windowClass;
        public IntPtr WindowHandle => _hwnd;
        
        public static void Initialize()
        {
            // Only initialize once
            if (AvaloniaLocator.Current.GetService<IPlatformUtils>() is {}) return;
            var pu = new WindowsPlatformUtils();
            AvaloniaLocator.CurrentMutable
                .Bind<IPlatformUtils>().ToConstant(pu)
                .Bind<WindowsPlatformUtils>().ToConstant(pu);
        }

        private void ReleaseUnmanagedResources()
        {
            if (_hwnd != IntPtr.Zero)
            {
                WinApi.DestroyWindow(_hwnd);
                _hwnd = IntPtr.Zero;
            }
            if (_windowClass != null)
            {
                WinApi.UnregisterClass(_windowClass, WinApi.GetModuleHandle(lpModuleName: null));
                _windowClass = null;
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~WindowsPlatformUtils()
        {
            ReleaseUnmanagedResources();
        }
    }
}