using System.Collections.Generic;

namespace XComponent.Common.UI.Grid.ViewModel
{
    public class ColumnFilterData
    {
        public string ColumnMappingName { get; set; }

        public List<FilterData> ColumnFilters { get; set; } 
    }
}
