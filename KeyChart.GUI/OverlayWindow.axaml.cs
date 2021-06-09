using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using static Avalonia.Controls.Design;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using KeyChart.GUI.Util;

namespace KeyChart.GUI
{
    public class OverlayWindow : Window
    {
        public OverlayWindow(): base(CreatePlatformWindow()) // 
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private static IWindowImpl CreatePlatformWindow()
        {
            if (!IsDesignMode)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new OverlayWindowImplWin32();
                }
            }

            return PlatformManager.CreateWindow();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ToggleVisible()
        {
            if (PlatformImpl is OverlayWindowImplWin32 ow)
            {
                ow.ToggleOverview();
            }
        }

        private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;
            if (PlatformImpl is OverlayWindowImplWin32 {ShowingOverview: true} ow)
            {
                ow.ToggleOverview();
            }
        }
    }
}