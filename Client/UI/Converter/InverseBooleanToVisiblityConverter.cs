using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Uniars.Client.UI.Converter
{
    public class InverseBooleanToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Hidden;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Hidden)
            {
                return true;
            }

            return false;
        }
    }
}
