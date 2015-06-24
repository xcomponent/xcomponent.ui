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
using System.Windows.Shapes;
using FirstFloor.ModernUI.Presentation;
using Syncfusion.Windows.Shared;

namespace XComponent.Common.UI.Docking
{
    /// <summary>
    /// Interaction logic for RenamingWindow.xaml
    /// </summary>
    public partial class RenamingWindow : ChromelessWindow
    {
        public RenamingWindow(string oldPanelTitle)
        {
            InitializeComponent();
            this.DataContext = this;
            PanelTitle = oldPanelTitle;            
            this.Loaded += (sender, args) => this.TitleTextBox.SelectAll();
            this.RenameCommand = new RelayCommand(o =>
            {
                this.DialogResult = true;
                this.Close();
            });
        }

        public static readonly DependencyProperty PanelTitleProperty = DependencyProperty.Register(
            "PanelPanelTitle", typeof (string), typeof (RenamingWindow), new PropertyMetadata(default(string)));

        public string PanelTitle
        {
            get { return (string) GetValue(PanelTitleProperty); }
            set { SetValue(PanelTitleProperty, value); }
        }

        public ICommand RenameCommand { get; set; }        
    }
}
