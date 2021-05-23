using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using KeyChart.Avalonia.Util;
using KeyChart.Keyboards;
using Avalonia.Win32;

namespace KeyChart.Avalonia
{
    public class App : Application
    {
        public static AppPaths Paths { get; } = AppPaths.ForApp<App>("KeyChart", "piksel");
        public static DataStore<Config> ConfigStore { get; } = new(Paths.ConfigPath / "config.json");
        public static Config Config => ConfigStore.Data;
        
        public static DataStore<KeyboardModel> KeyboardStore { get; } = new(Paths.ConfigPath / "keyboard.json");
        public static KeyboardModel? Keyboard => KeyboardStore.Data;

        
        public static QmkApiClient Qmk { get; } = new (Paths.CacheDir, new SystemTextJsonSerializer(
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true}));
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            
            
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel(),
                };
                
        
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class Config
    { 
        public PixelPoint? WindowPosition { get; set; }
        public PixelRect? Window { get; set; }
        
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }

        // public WindowConfig? Window { get; set; }
        //
        // public record WindowConfig(int Width, int Height);
    }
}