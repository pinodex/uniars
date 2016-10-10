using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace Uniars.Client.UI.Converter
{
    public class BooleanToSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return SelectionMode.Extended;
            }

            return SelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((SelectionMode)value == SelectionMode.Extended)
            {
                return true;
            }

            return false;
        }
    }
}
