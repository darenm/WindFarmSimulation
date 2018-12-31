using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace WindFarmDashboard.Converters
{
    public class FormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var formatString = parameter as string;
            return !string.IsNullOrEmpty(formatString) ? string.Format(formatString, value) : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}