using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Win32;
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
        
        public static readonly AttachedProperty<WindowIcon> IconProperty =
            AvaloniaProperty.RegisterAttached<NotifyIcon, Application, WindowIcon>(
                "Icon", default(WindowIcon), false, BindingMode.OneWay, null);
        
        public static void SetIcon(AvaloniaObject element, object parameter)
        {
            element.SetValue(IconProperty, parameter);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="IconProperty"/>.
        /// </summary>
        public static object GetIcon(AvaloniaObject element)
        {
            return element.GetValue(IconProperty);
        }
        
        public static readonly AttachedProperty<ContextMenu> MenuProperty =
            AvaloniaProperty.RegisterAttached<NotifyIcon, Application, ContextMenu>(
                "Icon", default(ContextMenu), false, BindingMode.OneWay, null);

        public static readonly AttachedProperty<bool> IsDefaultProperty = 
            AvaloniaProperty.RegisterAttached<MenuItem, bool>("IsDefault", typeof(NotifyIcon));
        

        public static void SetMenu(AvaloniaObject element, object parameter)
        {
            element.SetValue(MenuProperty, parameter);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="MenuProperty"/>.
        /// </summary>
        public static object GetMenu(AvaloniaObject element)
        {
            return element.GetValue(MenuProperty);
        }

        public static object GetIsDefault(IAvaloniaObject obj)
        {
            return obj.GetValue(IsDefaultProperty);
        }

        public static void SetIsDefault(IAvaloniaObject obj, object value)
        {
            obj.SetValue(IsDefaultProperty, value);
        }
    }
}