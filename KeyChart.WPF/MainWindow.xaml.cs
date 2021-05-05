using System;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeyboardCheatSheet.Models;

public class Key
{
    const int KeyU = 80;
    const int KeyP = 4;
    const int KeyB = KeyU + KeyP;

    public Key(int index, double x, double y, double w)
    {
        Index = index;
        Width = (KeyB * w) - KeyP;
        Margin = new Thickness(x * KeyB, y * KeyB, 0, 0);
    }

    public Thickness Margin { get; }
    public int Index { get; }
    public double Width { get; }
    public static double Height => KeyU;
}


namespace KeyboardCheatSheet
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private ImmutableArray<Button> keyButtons;
        private KeyboardConfig keyConf;
        private KeyboardInfo keyInfo;
        private KeyLayout keyMap;

        public ObservableCollection<Key> KeyLayout { get; } = new ObservableCollection<Key>();

        public LayerCollection Layers { get; } = new LayerCollection() { new Layer(0, Array.Empty<string>()) };

        public MainWindow()
        {
            InitializeComponent();


        }

        private void Grid_Initialized(object sender, EventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await using var keyConfFile = File.OpenRead("layout.json");
            keyConf = await JsonSerializer.DeserializeAsync<KeyboardConfig>(keyConfFile, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            // LayerList.Items.Clear();
            Layers.Clear();

            for (var i = 0; i < keyConf.Layers.GetLength(0); i++)
            {
                var keys = keyConf.Layers[i].Select(k => k.StartsWith("KC_") ? k.Substring(3) : k).ToArray();
                
                var layer = new Layer(i, keys);

                switch (i)
                {
                    case 1:
                    case 2:
                    layer.Display = false;
                    break;
                    case 3:
                        layer.AlignX = HorizontalAlignment.Left;
                        layer.AlignY = VerticalAlignment.Bottom;
                        break;
                    case 4:
                        layer.AlignX = HorizontalAlignment.Right;
                        layer.AlignY = VerticalAlignment.Bottom;
                        break;
                    case 5:
                        layer.AlignX = HorizontalAlignment.Right;
                        layer.AlignY = VerticalAlignment.Top;
                        break;
                }

                Layers.Add(layer);
                //LayerList.Items.Add(layer);
            }

            await using var keyInfoFile = File.OpenRead("info.json");
            keyInfo = await JsonSerializer.DeserializeAsync<KeyboardInfo>(keyInfoFile, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            //keyMap = keyInfo.Layouts[keyConf.Keymap];
            for (var i = 0; i < keyInfo.Layouts[keyConf.Layout].Layout.Length; i++)
            {
                var key = keyInfo.Layouts[keyConf.Layout].Layout[i];
                KeyLayout.Add(new Key(i, key.X, key.Y, key.W));
            }

            //keyButtons = KeyLayout.Select(key => new Button()
            //{
            //    Content = " ",
            //    DataContext = KeyLayout,

            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    VerticalAlignment = VerticalAlignment.Top,
            //}).ToImmutableArray();



            if (FindResource("KeyLayerTemplate") is DataTemplate dt)
            {
                // dt.
            }
                

            /*
            foreach (var button in keyButtons)
            {
                Buttons.Children.Add(button);
            }
            */
        }

        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            var index = LayerList.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            var keys = keyConf.Layers[index];
            for (var i = 0; i < keyButtons.Length; i++)
            {
                keyButtons[i].Content = keys[i];
            }
            */
        }
    }
}
