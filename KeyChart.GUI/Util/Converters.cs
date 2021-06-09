using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace KeyChart.GUI.Util
{
    public class Converters
    {
        public static ValueConditionConverter ValueConditionConverter = new();
        public static LabelAlignmentConverter LabelAlignmentConverter = new();
    }
    
    public class SizeVectortConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
        

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                (double x, double y) when targetType == typeof(Vector) => new Vector(x, y),
                (double x, double y) when targetType == typeof(Size) => new Size(x, y),
                Size(var x, var y) when targetType == typeof(Vector) => new Vector(x, y),
                Vector(var x, var y) when targetType == typeof(Size) => new Size(x, y),
                _ => null,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Convert(value, targetType, parameter, culture);

    }
}