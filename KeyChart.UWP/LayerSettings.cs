using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Windows.UI.Xaml;
using KeyChart.Keyboards;

namespace KeyChart
{



    public static class LayerSettingsExtensions
    {
        public static VerticalAlignment AlignY(this LayerStyle ls) 
            => ls.BaseLayer ? VerticalAlignment.Center 
                : ls.Position.HasFlag(LabelPosition.Bottom) ? VerticalAlignment.Bottom 
                    : VerticalAlignment.Top;
        public static HorizontalAlignment AlignX(this LayerStyle ls)
            => ls.BaseLayer ? HorizontalAlignment.Center
                : ls.Position.HasFlag(LabelPosition.Right) ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;
    }

    public class LayerCollection : ObservableCollection<LayerStyle>
    {
        public LayerCollection(){ }
        public LayerCollection(IEnumerable<LayerStyle> layers) : base(layers) { }
    }

    public class DesignLayerCollection : LayerCollection
    {

        public DesignLayerCollection(): base(new []
        {
            new LayerStyle(0),
            new LayerStyle(1),
            new LayerStyle(2)
        })
        {
        }
        
    }
}