using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
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
using KeyChart.GUI.Config;
using KeyChart.GUI.Util;
using KeyChart.GUI.Views;
using KeyChart.Keyboards;
using QMK = KeyChart.Keyboards.QMK;
using KeyChart.Platforms.Windows;
using KeyChart.Services;
using ReactiveUI;
using static KeyChart.GUI.Views.ConsoleViewHelpers;
using static KeyChart.GUI.Util.LogicStatic;
using Key = Avalonia.Input.Key;


namespace KeyChart.GUI
{
    public class MainWindow : Window
    {
        const int minHeight = 50;
        const int minWidth = 200;
        
        bool ConfigSaved = false;

        public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        private RowDefinition? ConsoleRow => this.FindControl<Grid>("ConsoleWrapper")
            ?.RowDefinitions.Skip(2).FirstOrDefault();

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

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            
            if(DataContext is not MainWindowViewModel vm) return;
            
            //vm.ActionButtonClicked = ReactiveCommand.Create<string>(OnActionButtonClicked);
        }

        private async void ActionButtonClicked(object sender, string action)
        {
            Debug.WriteLine($"ActionButton Invoked: {action}");
            await ButtonClicked(action);
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
                
                var consoleHeight = (int)(ConsoleRow?.ActualHeight ?? 200);
                var left = Position.X;
                var top = Position.Y;

                App.ConfigStore.Mutate(c =>
                {
                    c.MainWindow = new((int) Width, (int) Height, left, top, consoleHeight);
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
                await App.ConfigStore.LoadOrDefault(() => new AppConfig());
                await App.KeyboardStore.Load();
                
                if (App.Config.MainWindow is { } win)
                {
                    Given(win.Top > 0 && win.Left > 0)
                        .Do(() => Position = new PixelPoint(win.Left, win.Top));

                    Given(win.Height > MinHeight).Do(() => Height = win.Height);
                    Given(win.Width > MinWidth).Do(() => Width = win.Width);

                    ConsoleRow?.Let(cr => cr.Height = new GridLength(win.ConsoleHeight));
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
            await ButtonClicked((string?)b.Tag ?? "");
        }
        private async Task ButtonClicked(string action)
        {
            switch (action)
            {
                case "overlay":
                    App.ToggleOverlay();
                    break;
                case "import":
                    await ImportKeyboard();
                    break;
                case "reset":
                    ViewModel.Console.Lines.Clear();
                    break;
                case "dynkeys":
                    if (_viaComms is { })
                    {
                        if (App.Keyboard is not {} keyboard) return;
                        if (App.Keyboard.Info.MatrixSize is not {} matrix) return;
                        var matrixSize = (ushort)(matrix.Cols * matrix.Rows);
                        var layerCount = (byte)keyboard.LayerStyles.Count;
                        Debug.WriteLine($"Keys: {matrixSize}, Layers: {layerCount}");
                        _viaComms.GetDynamicKeys(matrixSize, layerCount);
                        //_viaComms.SendCommand(QMK.ViaCommand.dynamic_keymap_get_buffer, 0, (byte)(keymapOffset++ * 28), 28);

                    }
                    break;
                case "console":
                    ToggleConsole();
                    break;
                case "connect":
                    ConnectHID();
                    break;
                case "loading":
                    if (DataContext is MainWindowViewModel vm)
                    {
                        vm.Loaded = !vm.Loaded;
                    }
                    break;
                default:
                    await MessageBox.ShowDialog(this, $"Invalid button tag {action}");
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
            => Dispatcher.UIThread.InvokeAsync(() => ViewModel.Console?.AddLine(spans));
        
        HidComms? _hidComms;
        ViaHidComms? _viaComms;

        private void ToggleConsole()
        {
            if (ViewModel.Console is null)
            {
                ViewModel.Console = new ConsoleViewModel();
            }
            else
            {
                ViewModel.Console = null;
            }
            Debug.WriteLine(ViewModel.Console);
        }
        
        private void ConnectHID()
        {
            if(_hidComms is not null) return;
            _hidComms = HidComms.ConsoleHid;
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
                // ConsoleWriteLine( 
                //     MagentaText("RAW> "), 
                //     GrayText(msg)
                // );
            };
            _viaComms.DynamicKeysUpdated += ViaCommsOnDynamicKeysUpdated;
            _viaComms.RecievedLine += RawHidCommsOnRecievedLine;
            _viaComms.UpdateHidDevices(false);
            //rawHidComms.SendCommand(Encoding.UTF8.GetBytes("TEST"));
            //await MessageBox.ShowDialog(this, "Reset not implemented");
        }

        private void ViaCommsOnDynamicKeysUpdated(object? sender, EventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                ConsoleWriteLine(
                    MagentaText("DYN> "),
                    GrayText("Got updated keymap from HID. Updating labels..."));
                
                if (sender is not ViaHidComms via) return;
                if (App.Keyboard is not {} kbd) return;
                if (kbd.Info.MatrixSize is not {} matrixSize) return;

                // var width1u = KeyChart.Keyboards.Key.KeyWidth1U;
                // var lastKeyLeft = 0f;
                var dynKeys = via.DynamicKeys;
                // var sbRow = new StringBuilder();
                // foreach (var key in kbd.Layout)
                // {
                //     var offset = key.MatrixOffset;
                //     var keyCode = dynKeys.Count <= offset ? "- ? -" : $"{(QMK.KeyCode)dynKeys[offset],-5:G}";
                //     //var keyCode = $"{kc//kc == QMK.KeyCode.NO ? $"{kc,-5:G}" : " ??? ";
                //     // Debug.Write($"{key.Index} => @({key.Matrix.X,2}, {key.Matrix.Y,2}) [{offset,2}] {dynKey}");
                //     
                //     if (key.Bounds.Left < lastKeyLeft)
                //     {
                //         Debug.WriteLine(sbRow.ToString());
                //         sbRow.Clear();
                //     }
                //     lastKeyLeft = key.Bounds.Left;
                //     
                //     sbRow.Append($"{keyCode}".Substring(0, 5));
                //
                //     var keyWidth = key.Width / width1u;
                //     var keyPadAmount = 1 + (int)(keyWidth - 1) * 6;
                //
                //     sbRow.Append(new string(' ', keyPadAmount));
                //
                //     if (offset >= dynKeys.Count) break;
                // }
                //
                // Debug.WriteLine(sbRow.ToString());

                kbd.KeyMap.Source = "EEPROM";

                var index = 0;
                var layout = kbd.Layout;
                var layers = kbd.KeyMap.Layers;
                var layerOffset = matrixSize.Cols * matrixSize.Rows;
                var skipped = 0;
                for (int l = 0; l < layers.Length; l++)
                {
                    for (var k = 0; k < layers[l].Length; k++)
                    {
                        index = (layerOffset* l) + layout[k].MatrixOffset;
                        var currentKey = QMK.KeyCodes.NormalizeKeyName(layers[l][k]);
                        if (QMK.KeyCodes.GetString((QMK.KeyCode)dynKeys[index]) is { } newKey)
                        {
                            var oldKey = layers[l][k];
                            if (QMK.KeyCodes.Matches(oldKey, newKey))
                            {
                                skipped++;
                                continue;
                            }

                            if (skipped > 0)
                            {
                                ConsoleWriteLine(
                                    MagentaText("DYN> "),
                                    GrayText("Skipped"),
                                    CyanText($"{skipped}"),
                                    GrayText("identical key(s)")
                                );
                                skipped = 0;
                            }
                            
                            ConsoleWriteLine(
                                MagentaText("DYN> "),
                                CyanText($"#{index,3} "),
                                GrayText("Updated key"),
                                CyanText($"{k,2}"),
                                GrayText("of layer"),
                                CyanText($"{l}"),
                                GrayText("from"),
                                MagentaText($"{currentKey}"),
                                GrayText("to"),
                                MagentaText($"{newKey}")
                            );
                            
                            layers[l][k] = newKey;
                        }
                        else
                        {
                            ConsoleWriteLine(
                                MagentaText("DYN> "),
                                GrayText("Skipped setting key"),
                                CyanText($"{k}"),
                                GrayText("of layer"),
                                CyanText($"{l}"),
                                GrayText("(currently"),
                                MagentaText($"{layers[l][k]}"),
                                GrayText(") because of unknown EEPROM keycode"),
                                CyanText($"0x{dynKeys[index]:x4}")
                            );
                        }
                        // index++;
                    }
                }
                
                ConsoleWriteLine(
                    MagentaText("DYN> "),
                    GrayText("Rebuilding keyboard model..."));

                kbd.UpdateKeyLabels();
                await kbd.BuildKeyLayout();

                UpdateKeyViewModel(KeyboardViewModel.FromKeyboardModel(kbd));
                
                ConsoleWriteLine(
                    MagentaText("DYN> "),
                    GreenText("Keyboard model updated!"));
            });
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

        public ConsoleViewModel? Console
        {
            get => _consoleViewModel;
            set => this.RaiseAndSetIfChanged(ref _consoleViewModel, value);
        }

        ReactiveCommand<string, Unit> _actionButtonClicked;
        public ReactiveCommand<string, Unit> ActionButtonClicked
        {
            get => _actionButtonClicked;
            set => this.RaiseAndSetIfChanged(ref _actionButtonClicked, value);
        }
    }
}