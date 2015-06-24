using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace XComponent.Common.UI.Converter
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            try
            {
                bool boolValue = System.Convert.ToBoolean(value);
                if (boolValue)
                {
                    return parameter;
                }
                else
                {
                    return Application.Current.FindResource("XComponentFontBrush");
                }
            }
            catch(Exception)
            {
                //not boolean
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
