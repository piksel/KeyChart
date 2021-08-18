using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using KeyChart.GUI.Platforms.Windows;
using KeyChart.GUI.Util;

namespace KeyChart.GUI.Platforms
{
    public class NotifyIcon: AvaloniaObject
    {
        public static ITrayIcon? InitPlatformNotifyIcon(Window window, string tooltipText)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                
                if (window.PlatformImpl is HotkeyWindowImplWin32 win)
                {
                    return new WindowsTrayIcon() {
                        TooltipText = tooltipText,
                        Icon = window.Icon,
                        Window = win
                        };
                }
            }

            return null;
        }
        
        public static readonly AttachedProperty<WindowIcon?> IconProperty =
            AvaloniaProperty.RegisterAttached<NotifyIcon, Application, WindowIcon?>("Icon");
        
        public static void SetIcon(AvaloniaObject element, object parameter) => element.SetValue(IconProperty, parameter);

        /// <summary>
        /// Accessor for Attached property <see cref="IconProperty"/>.
        /// </summary>
        public static object? GetIcon(AvaloniaObject element) => element.GetValue(IconProperty);

        public static readonly AttachedProperty<ContextMenu?> MenuProperty =
            AvaloniaProperty.RegisterAttached<NotifyIcon, Application, ContextMenu?>("Menu");

        public static readonly AttachedProperty<bool> IsDefaultProperty = 
            AvaloniaProperty.RegisterAttached<MenuItem, bool>("IsDefault", typeof(NotifyIcon));
        

        public static void SetMenu(AvaloniaObject element, object parameter)
        {
            element.SetValue(MenuProperty, parameter);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="MenuProperty"/>.
        /// </summary>
        public static object? GetMenu(AvaloniaObject element) => element.GetValue(MenuProperty);

        public static object GetIsDefault(IAvaloniaObject obj) => obj.GetValue(IsDefaultProperty);

        public static void SetIsDefault(IAvaloniaObject obj, object value) => obj.SetValue(IsDefaultProperty, value);
    }
}