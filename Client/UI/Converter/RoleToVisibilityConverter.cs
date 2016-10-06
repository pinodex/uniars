using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

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
            Visibility visibility = (Visibility)value;

            if (App.Client != null && App.Client.CurrentUser != null && visibility == Visibility.Visible)
            {
                return App.Client.CurrentUser.Role;
            }

            return parameter;
        }
    }
}
