using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using XComponent.Common.UI.Grid.ViewModel;

namespace XComponent.Common.UI.Grid.Converter
{
    class ColumnChooserColumnsListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(values.Length == 1)
            {
                return values[0];
            }
            if(values.Length == 2)
            {
                IEnumerable<ColumnViewModel> allColumnsViewModel = values[0] as IEnumerable<ColumnViewModel>;
                IEnumerable<string> columnsToExclude = values[1] as IEnumerable<string>;
                if (columnsToExclude != null && allColumnsViewModel != null)
                {
                    return allColumnsViewModel.Where(x => !columnsToExclude.Contains(x.MappingName));
                }
                else
                {
                    return values[0];
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
