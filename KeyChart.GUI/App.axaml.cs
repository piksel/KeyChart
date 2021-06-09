using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using KeyChart.Keyboards;
using Avalonia.Win32;
using KeyChart.GUI.Platforms;
using KeyChart.GUI.Util;
using KeyChart.GUI.Views;
using KeyChart.Keyboards.QMK;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;

namespace KeyChart.GUI
{
    public class App : Application
    {
        private static KeyboardViewModel? keyboardViewModel;
        public static AppPaths Paths { get; } = AppPaths.ForApp<App>("KeyChart", "piksel");

        public static DataStore<Config> ConfigStore { get; } =
            new(Paths.ConfigPath / "config.json", new SystemTextJsonSerializer(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, 
                    WriteIndented = true, 
                    AllowTrailingCommas = true, 
                    ReadCommentHandling = JsonCommentHandling.Skip,
                }));

        public static Config Config => ConfigStore.Data;
        
        public static DataStore<KeyboardModel> KeyboardStore { get; } = new(Paths.ConfigPath / "keyboard.json");
        public static KeyboardModel? Keyboard => KeyboardStore.Data;

        
        public static QmkApiClient Qmk { get; } = new (Paths.CacheDir, new SystemTextJsonSerializer(
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true}));

        public static KeyboardViewModel? KeyboardViewModel
        {
            get => keyboardViewModel;
            set
            {
                keyboardViewModel = value;
                if (OverlayWindow is { } ow)
                {
                    ow.DataContext = value;
                }
            }
        }

        public static OverlayWindow? OverlayWindow { get; private set; }

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

                OverlayWindow = new OverlayWindow();
                OverlayWindow.Show();
                
                desktop.Exit += AppExit;

                InitNotifyIcon(desktop.MainWindow);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void AppExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            OverlayWindow?.Close();
        }

        private void InitNotifyIcon(Window mainWindow)
        {
            if(NotifyIcon.InitPlatformNotifyIcon(mainWindow, "KeyChart") is not {} ti) return;
            ti.DoubleClickCommand = ReactiveCommand.Create(mainWindow.ToggleMinimized);
            ti.SetIcon();
            ti.ContextMenu = new ContextMenu();
            ti.SetMenu(new List<PopupMenuItem>
            {
                new (mainWindow.ToggleMinimized, "KeyChart"),
                new (ToggleOverlay, "Toggle Overlay"),
                PopupMenuItem.Separator,
                new (() =>
                {
                    (Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
                }, "Exit KeyChart"),
            });
        }

        public static void ToggleOverlay()
        {
            if (OverlayWindow is { } ow)
            {
                ow.ToggleVisible();
            }
        }

        public static async Task<KeyboardModel?> ImportKeyboard()
        {
            await KeyboardStore.LoadOrDefault(() => null);

            var ofd = new OpenFileDialog();
            var files = await ofd.ShowAsync((Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
            if (files.FirstOrDefault() is not { } selectedFile) return null;
            await using var fileStream = File.OpenRead(selectedFile);
            
            if (await QmkClient.ParseCustomKeymap(fileStream) is not { } keymap)
            {
                throw new Exception("Invalid Config");
            }
            
            if (keymap.Keyboard is not { } keyboardName)
            {
                throw new Exception("Keyboard Name missing");
            }

            if ((await Qmk.KeyboardInfo(keyboardName))?.Keyboards is not { } keyboards )
            {
                throw new Exception( $"Could not retrieve keyboard info for \"{keyboardName}\"");
            }

            if (!keyboards.ContainsKey(keyboardName))
            {
                throw new Exception($"API did not include keyboard \"{keyboardName}\"");
            }

            var keyboardInfo = keyboards[keyboardName];
            
            var model = KeyboardModel.FromKeyboardKeyMap(keyboardInfo, keymap);
            await model.BuildKeyLayout();
            KeyboardStore.Data = model;
            await KeyboardStore.Save();
            
            KeyboardViewModel = KeyboardViewModel.FromKeyboardModel(model);
            if (OverlayWindow is { } ow)
            {
                ow.DataContext = OverlayWindow;
            }
            return model;
        }
    }

    public class Config
    {
        public WindowConfig? MainWindow { get; set; }
        //
        public record WindowConfig(int Width, int Height, int Left, int Top);
    }
}