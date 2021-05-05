using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using KeyChart.Keyboards;

namespace KeyChart
{
   

    public static class KeyExtensions
    {
        public static Thickness Margin(this Key key) => new(key.Bounds.Left, key.Bounds.Top, 0, 0);
    }


    public static class KeyLayerExtensions 
    {
        public static Style TextBlockStyle(this KeyLayer kl)
        {
            var s = new Style(typeof(TextBlock));
            if (kl.Symbol)
            {
                s.Setters.Add(new Setter(TextBlock.FontFamilyProperty, "Assets/fa6-thin.otf#Font Awesome 6 Pro"));
            }

            s.Setters.Add(new Setter(TextBlock.FontSizeProperty, kl.LayerStyle.BaseLayer ? 20 : 12));
            return s;
        }
    }

    public class KeyboardLayout : ObservableCollection<Key>
    {
        public KeyboardLayout() {}
        public KeyboardLayout(List<Key> list): base(list) { }

        public string Name { get; set; } = "";
    }


}
