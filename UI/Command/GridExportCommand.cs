using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Syncfusion.Windows.Controls.Grid;
using XComponent.Common.UI.GridExport;
using XComponent.Common.UI.Properties;

namespace XComponent.Common.UI.Command
{
    public static class GridExportCommand
    {
        static GridExportCommand()
        {
            CommandManager.RegisterClassCommandBinding(typeof(GridDataControl), new CommandBinding(ExportGrid, OnExecuteExportGrid, OnCanExecuteExportGrid));
        }

        public static RoutedCommand ExportGrid = new RoutedCommand("ExportGrid", typeof(GridDataControl));

        private static void OnExecuteExportGrid(object sender, ExecutedRoutedEventArgs args)
        {
            var dataGrid = args.Source as GridDataControl;
            if (dataGrid == null)
            {
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                FilterIndex = 5,
                Filter = Resources.ExportFilter
            };

            var fileType = GridExporterFactory.ExportedFileType.Excel97to2003;

            if (saveFileDialog.ShowDialog() == true && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                if (saveFileDialog.FilterIndex == 1)
                    fileType = GridExporterFactory.ExportedFileType.Excel97to2003;
                else if (saveFileDialog.FilterIndex == 2)
                    fileType = GridExporterFactory.ExportedFileType.Excel2007;
                else if (saveFileDialog.FilterIndex == 3)
                    fileType = GridExporterFactory.ExportedFileType.Excel2010;
                else if (saveFileDialog.FilterIndex == 4)
                    fileType = GridExporterFactory.ExportedFileType.Excel2013;
                else if (saveFileDialog.FilterIndex == 5)
                    fileType = GridExporterFactory.ExportedFileType.Csv;

                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    new GridExporterFactory().CreateGridExporter(fileType).ExportGrid(dataGrid, saveFileDialog.FileName);

                    //Message box confirmation to view the created spreadsheet.
                    if (MessageBox.MessageService.Current.ShowMessage(Resources.WannaViewFile, Resources.FileCreated,
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }   
                }
            }
        }

        private static void OnCanExecuteExportGrid(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }
    }
}
