using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XComponent.Common.UI.Converter
{
    public class IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
            {
                return (int?)null;
            }
            try
            {
                int result = System.Convert.ToInt32(value);
                return result;
            }
            catch
            {
                return (int?)null;
            }
        }
    }
}
