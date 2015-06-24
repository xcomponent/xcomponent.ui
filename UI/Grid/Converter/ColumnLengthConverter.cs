using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Syncfusion.Windows.Controls.Grid;

namespace XComponent.Common.UI.Grid.Converter
{
    public class ColumnLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double doubleValue = System.Convert.ToDouble(value);
                return new GridDataControlLength(doubleValue);
            }
            catch
            {
                return null;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                GridDataControlLength length = (GridDataControlLength)value;
                return length.Value;
            }
            catch
            {
                return Double.NaN;
            }
        }
    }
}
