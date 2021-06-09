using Avalonia;

namespace KeyChart.GUI.Platforms.Windows
{
    public class WindowsHotKeyHandler: IHotKeyManager
    {
        private WindowsHotKeyHandler(WindowsPlatformUtils wpu)
        {
            PlatformUtils = wpu;
            // wpu.WindowHandle
            
        }

        public WindowsPlatformUtils PlatformUtils { get; }

        public static void Initialize()
        {
            // Only initialize once
            if (AvaloniaLocator.Current.GetService<WindowsPlatformUtils>() is not { } wpu)
            {
                wpu = new WindowsPlatformUtils();
                AvaloniaLocator.CurrentMutable.Bind<WindowsPlatformUtils>().ToConstant(wpu);
            }

            var whkh = new WindowsHotKeyHandler(wpu);

            AvaloniaLocator.CurrentMutable
                .Bind<IHotKeyManager>().ToConstant(whkh);
        }
        
        
    }
}