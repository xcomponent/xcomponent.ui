using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;

namespace XComponent.Common.UI.GridExport
{
    internal class ExcelGridExporter : IGridExporter
    {
        private readonly ExcelVersion _excelVersion;

        public ExcelGridExporter(ExcelVersion excelVersion = ExcelVersion.Excel2013)
        {
            _excelVersion = excelVersion;
        }

        public void ExportGrid(GridDataControl grid, string filePath)
        {
            if (grid == null)
            {
                return;
            }

            grid.ExportToExcel(filePath, _excelVersion);
        }
    }
}
