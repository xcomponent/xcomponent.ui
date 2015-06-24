using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XComponent.Common.UI.Converter
{
    public class NotBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Invert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Invert(value);
        }

        private static object Invert(object value)
        {
            try
            {
                if (value == null)
                    return null;
                bool bvalue = System.Convert.ToBoolean(value);
                return !bvalue;
            }
            catch
            {
                return null;
            }
        }
    }
}
