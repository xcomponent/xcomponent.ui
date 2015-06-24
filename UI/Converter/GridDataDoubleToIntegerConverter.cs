using System;
using System.Globalization;
using System.Windows.Data;

namespace XComponent.Common.UI.Converter
{
    [ValueConversion(typeof(double), typeof(int))]
    public class DoubleToIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)System.Convert.ToInt32(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
