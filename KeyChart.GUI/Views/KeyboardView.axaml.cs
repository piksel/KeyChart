using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using KeyChart.GUI.Util;
using KeyChart.Keyboards;
using ReactiveUI;
using QMK = KeyChart.Keyboards.QMK;
using A = Avalonia;

namespace KeyChart.GUI.Views
{
    public class KeyboardView : UserControl
    {
        public KeyboardView()
        {
            Debug.WriteLine("Loading Keyboard View");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private static async Task<string> StreamSha256(Stream stream) 
            => string.Concat(SHA256.Create().ComputeHash(stream).Select(b => $"{b:x2}"));
        
        private async void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            KeyboardModel? model;
            try
            {
                model = await App.ImportKeyboard();
                if (model is null) return;
            }
            catch (Exception x)
            {
                await MessageBox.ShowDialog(this.FindAncestorOfType<Window>(), x.Message, "Error importing config");
                return;
            }

            DataContext = new KeyboardViewModel
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
    }

    public class KeyboardViewModel : ViewModelBase
    {
        public IReadOnlyList<LayerStyle> LayerStyles { get; set; } = Array.Empty<LayerStyle>();
        
        private IReadOnlyList<Key> _layout = new List<Key>();
        public IReadOnlyList<Key> Layout
        {
            get => _layout;
            set
            {
                this.RaiseAndSetIfChanged(ref _layout, value);
                UpdateSize();
            }
        }
        
        public QMK.KeyMap KeyMap { get; set; } = new();

        private QMK.KeyboardInfo _info = new();
        public QMK.KeyboardInfo Info
        {
            get => _info;
            set
            {
                this.RaiseAndSetIfChanged(ref _info, value);
                // UpdateSize();
            }
        }

        private string _name = "";
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private bool _keyboardSelected;

        public bool KeyboardSelected
        {
            get => _keyboardSelected;
            set => this.RaiseAndSetIfChanged(ref _keyboardSelected, value);
        }

        private string _source = "";
        public string Source
        {
            get => _source;
            set => this.RaiseAndSetIfChanged(ref _source, value);
        }

        private Size _size = new Size(1, 1);

        private double _height = 1;
        private double _width = 1;

        public double Height
        {
            get => _height;
            private set => this.RaiseAndSetIfChanged(ref _height, value);
        }
        
        public double Width
        {
            get => _width;
            private set => this.RaiseAndSetIfChanged(ref _width, value);
        }
        
        private void UpdateSize()
        {
            Debug.WriteLine("KeyMaps: {0}, Keys: {1}", _info.Keymaps.Count , _layout.Count);
            if (_layout.Count <= 0) return;
            //this.RaisePropertyChanging(nameof(Height));
            //this.RaisePropertyChanging(nameof(Width));
            //var layout = Info.Layouts[KeyMap.Layout ?? "LAYOUT"].Layout;

            var maxRight = .0;
            var maxBottom = .0;
            foreach (var key in _layout)
            {
                var bounds = key.Bounds;
                maxRight = Math.Max(maxRight, bounds.Right);
                maxBottom = Math.Max(maxBottom, bounds.Bottom);
            }

            Height = maxBottom;
            Width = maxRight;
            Debug.WriteLine("New size: {0}, {1}", Width, Height);
            //_size = new Size(maxRight, maxBottom);
            //this.RaisePropertyChanged(nameof(Height));
            //this.RaisePropertyChanged(nameof(Width));
        }

        Rect _bounds;
        public Rect Bounds
        {
            get => _bounds;
            set => this.RaiseAndSetIfChanged(ref _bounds, value);
        }

        public static KeyboardViewModel FromKeyboardModel(KeyboardModel model)
        =>  new()
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
}