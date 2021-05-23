using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KeyChart.Avalonia.Util
{
    public class ValueConditionConverter: IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value is true ? parameter : default;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();
    }
}