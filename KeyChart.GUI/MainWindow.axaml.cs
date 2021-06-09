using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using static Avalonia.Controls.Design;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using DynamicData.Binding;
using KeyChart.GUI.Util;
using KeyChart.GUI.Views;
using KeyChart.Keyboards;
using QMK = KeyChart.Keyboards.QMK;
using KeyChart.Platforms.Windows;
using KeyChart.Services;
using ReactiveUI;
using static KeyChart.GUI.Views.ConsoleViewHelpers;
using Key = Avalonia.Input.Key;

namespace KeyChart.GUI
{
    public class MainWindow : Window
    {
        bool ConfigSaved = false;

        public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel; 

        public MainWindow(): base(CreatePlatformWindow())
        {
            if (PlatformImpl is HotkeyWindowImplWin32 hw)
            {
                hw.RegisterHotkey(HotkeyService.HotkeyAction.ShowOverlay, KeyModifiers.None, Key.Pause);
                hw.HotkeyPressed += HwOnHotkeyPressed;
            }
            
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property != WindowStateProperty || !e.IsEffectiveValueChange) return;
            if (e.NewValue is not Avalonia.Controls.WindowState newState) return;
            ShowInTaskbar = newState != WindowState.Minimized;
        }

        private static IWindowImpl CreatePlatformWindow()
        {
            if (!IsDesignMode)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new HotkeyWindowImplWin32();
                }
            }

            return PlatformManager.CreateWindow();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void HwOnHotkeyPressed(object? sender, HotkeyService.HotkeyAction e)
        {
            switch (e)
            {
                case HotkeyService.HotkeyAction.ShowOverlay:
                    App.ToggleOverlay();
                    break;
                default:
                    Debug.WriteLine("Unknown hotkey triggered: {0:G} ({0})", e);
                    break;
            }
        }

        private async void WindowBase_OnActivated(object? sender, EventArgs e)
        {
           
        }

        private  void WindowBase_OnDeactivated(object? sender, EventArgs e)
        {

        }

        private async void Window_OnClosing(object? sender, CancelEventArgs e)
        {
            // If we don't need to save config exit without interrupting the close
            if(ConfigSaved || !IsVisible) return;

            try
            {
                e.Cancel = true;

                App.ConfigStore.Mutate(c =>
                {
                    c.MainWindow = new Config.WindowConfig((int) Width, (int) Height, Position.X, Position.Y);
                });

                // App.Config.Window = new Config.WindowConfig((int)Width, (int)Height);
                await App.ConfigStore.Save();
            }
            finally
            {
                // Close regardless of outcome
                ConfigSaved = true;
                Close();
            }
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

                if (IsDesignMode) return;
                
                var _ = App.Paths.ConfigDir;
                await App.ConfigStore.LoadOrDefault(() => new Config());
                await App.KeyboardStore.Load();
                
                if (App.Config.MainWindow is { } win) { 
                    if (win.Top > 0 && win.Left > 0)
                    {
                        Position = new PixelPoint(win.Left, win.Top);
                    }

                    Height = win.Height > 50 ? win.Height : Height;
                    Width = win.Width > 200 ? win.Width : Width;
                }


                if (App.Keyboard is not { } model)
                {
                    return;
                }

                await model.BuildKeyLayout();
                var keyboardViewModel = KeyboardViewModel.FromKeyboardModel(model);
                UpdateKeyViewModel(keyboardViewModel);
        
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

        private void UpdateKeyViewModel(KeyboardViewModel keyboardViewModel)
        {
            var kv = this.FindControl<KeyboardView>(nameof(KeyboardView));
            kv.DataContext = keyboardViewModel;
                


            App.KeyboardViewModel = keyboardViewModel;
        }

        private async void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            switch (b.Tag)
            {
                case "overlay":
                    App.ToggleOverlay();
                    break;
                case "import":
                    await ImportKeyboard();
                    break;
                case "reset":
                    if (_viaComms is { })
                    {
                        _viaComms.SendCommand(QMK.ViaCommand.get_protocol_version);
                    }
                    break;
                case "console":
                    ToggleConsole();
                    break;
                case "loading":
                    if (DataContext is MainWindowViewModel vm)
                    {
                        vm.Loaded = !vm.Loaded;
                    }
                    break;
                default:
                    await MessageBox.ShowDialog(this, $"Invalid button tag {b.Tag}");
                    break;
            }
        }

        private async Task ImportKeyboard()
        {
            KeyboardModel? model;
            try
            {
                model = await App.ImportKeyboard();
            }
            catch (Exception x)
            {
                await MessageBox.ShowDialog(this, x.Message, "Error importing config");
                return;
            }
            if(model is null) return;
            
            UpdateKeyViewModel(KeyboardViewModel.FromKeyboardModel(model));
        }

        public void ConsoleWriteLine(params TextSpan[] spans)
            => Dispatcher.UIThread.InvokeAsync(() => ViewModel.Console.AddLine(spans));
        
        HidComms? _hidComms;
        ViaHidComms? _viaComms;
        private void ToggleConsole()
        {
            if(_hidComms is not null) return;
            _hidComms = HidComms.ConsoleHid;
            ViewModel.Console = new ConsoleViewModel();
            _hidComms.MessageLogged += (_, msg) =>
            {
                Debug.WriteLine(msg, "HID]");
                ConsoleWriteLine( 
                    BlueText("HID> "), 
                    GrayText(msg)
                );
            };
            _hidComms.RecievedLine += HidCommsOnRecievedLine;
            _hidComms.UpdateHidDevices(false);


            if (_viaComms is not null) return;
            _viaComms = new ViaHidComms();
            // ViewModel.Console = new ConsoleViewModel();
            _viaComms.MessageLogged += (_, msg) =>
            {
                Debug.WriteLine(msg, "RAW]");
                ConsoleWriteLine( 
                    MagentaText("RAW> "), 
                    GrayText(msg)
                );
            };
            _viaComms.RecievedLine += RawHidCommsOnRecievedLine;
            _viaComms.UpdateHidDevices(false);
            //rawHidComms.SendCommand(Encoding.UTF8.GetBytes("TEST"));
            //await MessageBox.ShowDialog(this, "Reset not implemented");
        }

        private void HidCommsOnRecievedLine(object? sender, string line)
        {
            try
            {
                if (line.StartsWith("keyboard_report:"))
                {
                    var kbBytes = line.Split(' ').Skip(1)
                        .Select(s => byte.TryParse(s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out var b) ? b : 0) 
                            .ToImmutableArray();
                    var stateA = (QMK.Modifier) kbBytes[0];
                    var stateB = kbBytes[1];
                    var pressed = kbBytes.Skip(2).TakeWhile(b => b > 0).Select(b => (QMK.KeyCode)b).ToImmutableArray();
                    var pressedKeys = string.Join(", ", pressed);
                    Debug.WriteLine(
                        $"Keyboard State Mods: {stateA:F}, Byte2: {stateB}, Pressed: {string.Join(", ", pressed)} ({pressed.Length} key(s))",
                        "HID>");
                    ConsoleWriteLine(
                        BlueText("KBDBG> "),
                        GrayText("Mods: "),
                        MagentaText($"{stateA:F}"),
                        GrayText("Pressed: "),
                        CyanText(pressedKeys)
                    );
                }
                else
                {
                    ConsoleWriteLine(
                        BlueText("MSG> "),
                        GrayText(line.Replace("\t", "    "))
                    );
                    Debug.WriteLine(line, "MSG>");
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
            }
        }
        
        private void RawHidCommsOnRecievedLine(object? sender, string line)
        {
            try
            {
                if (line.StartsWith("keyboard_report:"))
                {

                }
                else
                {
                    ConsoleWriteLine(
                        GreenText("VIA> "),
                        GrayText(line)
                    );
                    Debug.WriteLine(line, "VIA>");
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
            }
        }

        private void TopLevel_OnClosed(object? sender, EventArgs e)
        {
            App.OverlayWindow?.Close();
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
        
        private ConsoleViewModel? _consoleViewModel;

        public ConsoleViewModel Console
        {
            get => _consoleViewModel;
            set => this.RaiseAndSetIfChanged(ref _consoleViewModel, value);
        }
    }
}