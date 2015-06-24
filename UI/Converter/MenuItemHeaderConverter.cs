using System;
using System.Globalization;
using System.Windows.Data;
using XComponent.Common.UI.Properties;

namespace XComponent.Common.UI.Converter
{
    public class MenuItemHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var title = value as string;
            return Resources.SendToMenuItemLabel + title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
