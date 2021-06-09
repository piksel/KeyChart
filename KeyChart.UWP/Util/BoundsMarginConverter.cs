using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#nullable enable

namespace KeyChart.Util
{
    public class BoundsMarginConverter: IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
            => value is RectangleF rf && targetType == typeof(Thickness) ? rf.ToThickness() : null;

        public object? ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public static class RectangleFExtensions
    {
        public static Thickness ToThickness(this RectangleF r) => new(r.Left, r.Top, 0, 0);
    }
}
