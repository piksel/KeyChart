using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using KeyChart.GUI.Platforms.Windows;

namespace KeyChart.GUI.Platforms
{
    public static class AppBuilderUtilityExtensions
    {
        public static TAppBuilder UseHotKeys<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            var os = builder.RuntimePlatform.GetRuntimeInfo().OperatingSystem;
            
            if (os == OperatingSystemType.WinNT)
            {
                WindowsPlatformUtils.Initialize();
                WindowsHotKeyHandler.Initialize();
            }
            else if(os==OperatingSystemType.OSX)
            {
                // LoadUtilsMacOs(builder);
            }
            else
            {
                // LoadUtilsX11(builder);
            }

            return builder;
        }
    }
}