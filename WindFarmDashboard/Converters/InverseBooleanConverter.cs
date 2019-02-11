using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WindFarmDashboard.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(bool))
                return !(bool)value;

            if (targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }

            throw new InvalidOperationException("Must be a bool or Visibility");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}