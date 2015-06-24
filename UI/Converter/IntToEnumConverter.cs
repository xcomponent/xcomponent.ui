using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace XComponent.Common.UI.Converter
{
    public class IntToEnumConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty EnumValuesListProperty = DependencyProperty.Register("EnumValuesList", typeof(IEnumerable), typeof(IntToEnumConverter));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int && EnumValuesList != null)
            {
                int intValue = (int)value;
                Type enumType = EnumValuesList.Cast<object>().First().GetType();
                return Enum.ToObject(enumType, intValue);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return System.Convert.ToInt32(value);
            }
            return -1;
        }

        public IEnumerable EnumValuesList
        {
            get { return (IEnumerable)GetValue(EnumValuesListProperty); }
            set { SetValue(EnumValuesListProperty, value); }
        }
    }
}
