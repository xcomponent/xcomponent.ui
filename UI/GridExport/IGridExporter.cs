using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syncfusion.Windows.Controls.Grid;

namespace XComponent.Common.UI.GridExport
{
    public interface IGridExporter
    {
        void ExportGrid(GridDataControl grid, string filePath, string dateTimeFormat = "");
    }
}
