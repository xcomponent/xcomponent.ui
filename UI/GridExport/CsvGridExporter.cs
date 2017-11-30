using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using Syncfusion.Linq;
using Syncfusion.Windows.Controls.Grid;

namespace XComponent.Common.UI.GridExport
{
    internal class CsvGridExporter : IGridExporter
    {
        private readonly char _delimiter;

        public CsvGridExporter(char delimiter = ';')
        {
            _delimiter = delimiter;
        }

        public void ExportGrid(GridDataControl grid, string filePath, string dateTimeFormat = "")
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string exportString = string.Empty;

                var sb = new StringBuilder();
                bool checkAccess = grid.Dispatcher.CheckAccess();

                ICollectionView collectionView = null;
                GridDataVisibleColumns visibleColumns = null;

                if (checkAccess)
                {
                    visibleColumns = grid.VisibleColumns;
                    collectionView = CollectionViewSource.GetDefaultView(grid.ItemsSource);
                }
                else
                {
                    grid.Dispatcher.Invoke(() => visibleColumns = grid.VisibleColumns);
                    grid.Dispatcher.Invoke(() => collectionView = CollectionViewSource.GetDefaultView(grid.ItemsSource));
                }

                var headerLineBuilder = new StringBuilder();

                // We only want to export visible columns..
                visibleColumns.ForEach(c =>
                {
                    if (!c.IsHidden)
                        headerLineBuilder.Append(c.HeaderText + _delimiter);
                });

                // Remove trailing delimiter..
                headerLineBuilder.Remove(headerLineBuilder.Length - 1, 1);

                sb.AppendLine(headerLineBuilder.ToString());

                foreach (object o in collectionView)
                {
                    var itemLineBuilder = new StringBuilder();

                    foreach (var column in visibleColumns)
                    {
                        if (column.IsHidden)
                        {
                            // We only want to export visible columns..
                            continue;
                        }

                        string propertyValue = string.Empty;

                        var value = GetObjectPropertyValueFromColumnMappingName(o, column.MappingName);

                        if (value != null)
                        {
                            if (value is DateTime && !string.IsNullOrEmpty(dateTimeFormat))
                            {
                                propertyValue = ((DateTime)value).ToString(dateTimeFormat);
                            }
                            else
                            {
                                propertyValue = value.ToString();
                            }
                        }

                        itemLineBuilder.Append(propertyValue + _delimiter);
                    }

                    // Remove trailing delimiter..
                    itemLineBuilder.Remove(itemLineBuilder.Length - 1, 1);

                    sb.AppendLine(itemLineBuilder.ToString());
                }

                exportString = sb.ToString();

                using (var sw = new StreamWriter(File.Open(filePath, FileMode.Create), Encoding.Default))
                {
                    sw.Write(exportString);
                }
            }
        }

        private object GetObjectPropertyValueFromColumnMappingName(object o, string mappingName)
        {
            var propertySequence = mappingName.Split('.');

            PropertyInfo propertyInfo = null;

            if (propertySequence.Count() != 0)
            {
                propertyInfo = o.GetType().GetProperty(propertySequence[0]);

                for (int i = 0; i < propertySequence.Count() - 1; i++)
                {
                    o = propertyInfo.GetValue(o);

                    if (o != null)
                    {
                        propertyInfo = o.GetType().GetProperty(propertySequence[i + 1]);
                    }
                }
            }

            return (propertyInfo != null) ? propertyInfo.GetValue(o) : null;
        }
    }
}
