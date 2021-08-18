using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.ReactiveUI;

namespace KeyChart.GUI
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {


            BuildAvaloniaApp()
                .AfterPlatformServicesSetup(ab =>
                {
                    var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    
                    var availableAssets = assetLoader.GetAssets(
                        new Uri("avares://KeyChart/Assets/Fonts"),
                        new Uri("avares://KeyChart/Styles.axaml"));

                    var loadedFonts = string.Join("\n", availableAssets.Select(a => a.AbsolutePath));

      
                    Debug.WriteLine($"FONTS: {loadedFonts}");
                    
                    
                })
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                //.UseHotKeys()
                //.UseStatusIcon()
                .LogToTrace()
                .UseReactiveUI();
    }
}