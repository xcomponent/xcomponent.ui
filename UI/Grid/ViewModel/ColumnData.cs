using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XComponent.Common.UI.Grid.ViewModel
{
    public class ColumnData
    {

        public string MappingName { get; set; }

        public string DisplayName { get; set; }

        public double Width { get; set; }

        public bool IsHidden { get; set; }

        public int Position { get; set; }
    }
}
