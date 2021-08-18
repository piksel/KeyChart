using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using KeyChart.Keyboards;
using Piksel.TextSymbols;

#nullable enable

namespace KeyChart.GUI.Util
{

    public class LabelAlignmentConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
        

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            if (value is not LabelPosition lp) return null;

            if (targetType == typeof(VerticalAlignment)) return lp.ToVerticalAlignment();
            if (targetType == typeof(HorizontalAlignment)) return lp.ToHorizontalAlignment();

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
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
    
    public class TextSymbolText
    {
        public TextSymbolText(Fa6 symbol)
        {
            Symbol = symbol;
        }

        public TextSymbol Symbol { get; }

        public object? ProvideValue(IServiceProvider serviceProvider) => Symbol.Text;
    }


}
