using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace KeyboardCheatSheet
{
    public class LayerKeyIndexConverter : IMultiValueConverter
    {
        private static readonly Dictionary<string, string> keyNames = new Dictionary<string, string>()
        {
            {"TRNS", " "}
        };

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return "!2";

            if (!(values[1] is int idx) || !(values[0] is string[] keys))
                return "null";

            var key = keys[idx];

            return keyNames.ContainsKey(key) ? keyNames[key] : key;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
