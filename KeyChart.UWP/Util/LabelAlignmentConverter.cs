using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using KeyChart.Keyboards;

#nullable enable

namespace KeyChart.Util
{
    [MarkupExtensionReturnType(ReturnType = typeof(IValueConverter))]
    public class LabelAlignmentConverter : MarkupExtension, IValueConverter
    {
        protected override object ProvideValue() => this;

        public object? Convert(object? value, Type targetType, object parameter, string language)
        {
            if (value is not LabelPosition lp) return null;

            if (targetType == typeof(VerticalAlignment)) return lp.ToVerticalAlignment();
            if (targetType == typeof(HorizontalAlignment)) return lp.ToHorizontalAlignment();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();

    }

    public static class LabelPositionExtensions
    {
        public static VerticalAlignment ToVerticalAlignment(this LabelPosition p)
            => p.HasFlag(LabelPosition.Center) ? VerticalAlignment.Center
                : p.HasFlag(LabelPosition.Bottom) ? VerticalAlignment.Bottom
                : VerticalAlignment.Top;
        public static HorizontalAlignment ToHorizontalAlignment(this LabelPosition p)
            => p.HasFlag(LabelPosition.Center) ? HorizontalAlignment.Center
                : p.HasFlag(LabelPosition.Right) ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;
    }


}
