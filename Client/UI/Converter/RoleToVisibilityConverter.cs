using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Uniars.Client.UI.Converter
{
    public class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string role = parameter as string;

            if (App.Client != null && App.Client.CurrentUser != null && role == App.Client.CurrentUser.Role)
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
