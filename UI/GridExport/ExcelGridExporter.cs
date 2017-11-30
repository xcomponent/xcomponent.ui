using System;
using System.Collections.Generic;
using System.Globalization;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.Windows.Shared;
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

        public void ExportGrid(GridDataControl grid, string filePath, string dateTimeFormat = "")
        {
            if (grid == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(dateTimeFormat))
            {
                var syncfusionDateFormat = dateTimeFormat.Replace('f', '0');

                grid.ExportToExcel(filePath, _excelVersion, (sender, args) =>
                {
                    args.Style.DateTimeEdit.DateTimePattern = DateTimePattern.CustomPattern;
                    args.Style.DateTimeEdit.CustomPattern = syncfusionDateFormat;
                });
            }
            else
            {
                grid.ExportToExcel(filePath, _excelVersion);
            }
        }
    }
}
