using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

#nullable enable

namespace KeyChart.Util
{
    [MarkupExtensionReturnType(ReturnType = typeof(IValueConverter))]
    public class NullVisibilityConverter : MarkupExtension, IValueConverter
    {
        public bool VisibleWhenNull { get; set; }

        protected override object ProvideValue() => this;

        public object Convert(object? value, Type targetType, object parameter, string language) 
            => value == null ^ VisibleWhenNull ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, string language) 
            => throw new NotImplementedException();
    }
}
