using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using KeyChart.Avalonia.Views;
using KeyChart.Keyboards;
using KeyChart.Services;
using ReactiveUI;

namespace KeyChart.Avalonia
{
    public class MainWindow : Window
    {
        bool ConfigSaved = false;
        
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
           
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void WindowBase_OnActivated(object? sender, EventArgs e)
        {
           
        }

        private  void WindowBase_OnDeactivated(object? sender, EventArgs e)
        {

        }

        private async void Window_OnClosing(object? sender, CancelEventArgs e)
        {
            if(ConfigSaved) return;
            
            e.Cancel = true;
            
            App.ConfigStore.Data = new Config
            {
                WindowHeight = Height,
                WindowWidth = Width,
                WindowPosition = Position,
                Window = new PixelRect(Position.X, Position.Y, (int)Width, (int)Height),
            };

            // App.Config.Window = new Config.WindowConfig((int)Width, (int)Height);
            await App.ConfigStore.Save();
            ConfigSaved = true;
            Close();
        }

        private async void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            await App.ConfigStore.Save();
        }

        private async void StyledElement_OnInitialized(object? sender, EventArgs e)
        {
            //this.Icon = new WindowIcon()
            
            try
            {
                //ExtendClientAreaToDecorationsHint = true;
                //ExtendClientAreaTitleBarHeightHint = -1;
                //ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                
                var _ = App.Paths.ConfigDir;
                await App.ConfigStore.LoadOrDefault(() => new Config());
                await App.KeyboardStore.Load();

                if (App.Config.WindowHeight is { } height)
                {
                    Height = height;
                }

                if (App.Config.WindowWidth is { } width)
                {
                    Width = width;
                }


                if (App.Keyboard is not { } model)
                {
                    return;
                }

                await model.BuildKeyLayout();
                var kv = this.FindControl<KeyboardView>(nameof(KeyboardView));
                kv.DataContext = new KeyboardViewModel
                {
                    Info = model.Info,
                    Layout = model.Layout,
                    Name = model.Name,
                    Source = model.Source,
                    KeyMap = model.KeyMap,
                    LayerStyles = model.LayerStyles,
                    KeyboardSelected = true,
                };
                
                
            }
            finally
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    // await Task.Delay(TimeSpan.FromSeconds(2));
                    viewModel.Loaded = true;
                }
            }
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private bool _loaded;

        public bool Loaded
        {
            get => _loaded;
            set => this.RaiseAndSetIfChanged(ref _loaded, value);
        }

        private KeyboardViewModel _keyboardViewModel = new();

        public KeyboardViewModel KeyboardViewModel
        {
            get => _keyboardViewModel;
            set => this.RaiseAndSetIfChanged(ref _keyboardViewModel, value);
        }
    }
}