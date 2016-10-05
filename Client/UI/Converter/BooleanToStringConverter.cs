using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Uniars.Client.UI.Converter
{
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] values = (parameter as string).Split('|');

            if ((bool)value)
            {
                return values[0];
            }

            return values[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] values = (parameter as string).Split('|');

            if ((string)value == values[0])
            {
                return true;
            }

            return false;
        }
    }
}
