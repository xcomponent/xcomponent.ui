using System;
using System.Globalization;
using System.Windows.Data;

namespace XComponent.Common.UI.Wpf
{
    public class CheckEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strVal = value as string;
            if (string.IsNullOrWhiteSpace(strVal))
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
