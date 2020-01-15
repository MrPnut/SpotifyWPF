using System;
using System.Globalization;
using System.Windows.Data;

namespace SpotifyWPF.Converter
{
    public class RadioButtonStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)parameter == (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value ? parameter : null;
        }
    }
}
