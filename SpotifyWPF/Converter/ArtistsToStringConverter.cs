using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using SpotifyAPI.Web;


namespace SpotifyWPF.Converter
{
    public class ArtistsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is IList<SimpleArtist>) ? null : 
                string.Join(",", ((IList<SimpleArtist>) value).Select(a => a.Name).ToList());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
