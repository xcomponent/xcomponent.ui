using Syncfusion.XlsIO;

namespace XComponent.Common.UI.GridExport
{
    public class GridExporterFactory
    {
        public enum ExportedFileType
        {
            Excel97to2003,
            Excel2007,
            Excel2010,
            Excel2013,
            Csv
        }

        public IGridExporter CreateGridExporter(ExportedFileType fileType)
        {
            switch (fileType)
            {
                case ExportedFileType.Excel97to2003:
                    return new ExcelGridExporter(ExcelVersion.Excel97to2003);
                case ExportedFileType.Excel2007:
                    return new ExcelGridExporter(ExcelVersion.Excel2007);
                case ExportedFileType.Excel2010:
                    return new ExcelGridExporter(ExcelVersion.Excel2010);
                case ExportedFileType.Excel2013:
                    return new ExcelGridExporter(ExcelVersion.Excel2013);
                case ExportedFileType.Csv:
                    return new CsvGridExporter();
            }

            return null;
        }
    }
}
