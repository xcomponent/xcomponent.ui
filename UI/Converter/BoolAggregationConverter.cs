using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XComponent.Common.UI.Converter
{
    internal class BoolAggregationConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 0)
            {
                return null;
            }

            bool? result = (bool)values[0];

            for(int i=0; i<values.Length; i++)
            {
                bool isHidden = (bool)values[i];
                if(isHidden != result)
                {
                    result = null;
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
            {
                return null;
            }
            bool boolvalue = (bool)value;
            object[] result = new object[targetTypes.Length];
            for (int i = 0; i < targetTypes.Length; i++)
            {
                result[i] = boolvalue;
            }
            return result;
        }
    }
}
