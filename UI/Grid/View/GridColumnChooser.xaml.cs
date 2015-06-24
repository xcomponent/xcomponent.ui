using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using XComponent.Common.UI.Grid.ViewModel;
using XComponent.Common.UI.MessageBox;

namespace XComponent.Common.UI.Grid.View
{
    /// <summary>
    /// Interaction logic for GridColumnChooser.xaml
    /// </summary>
    public partial class GridColumnChooser : ChromelessWindow
    {
        public static readonly DependencyProperty ColumnsVisibilityProperty = DependencyProperty.Register("ColumnsVisibility", typeof(ColumnsVisualizationViewModel), typeof(GridColumnChooser));

        public GridColumnChooser()
        {
            InitializeComponent();
            if(System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                ColumnsVisibility = new ColumnsVisualizationViewModel();
            }
        }

        public ColumnsVisualizationViewModel ColumnsVisibility
        {
            get { return (ColumnsVisualizationViewModel)GetValue(ColumnsVisibilityProperty); }
            set { SetValue(ColumnsVisibilityProperty, value); }
        }

        private void ModernButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        private void ChromelessWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(ColumnsVisibility.AllColumnsVisible.HasValue && !ColumnsVisibility.AllColumnsVisible.Value)
            {
                XCMessageBox.ShowMessage(Properties.Resources.AtLeastOneColumnMsg, Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                e.Cancel = true;
            }
        }
    }
}
