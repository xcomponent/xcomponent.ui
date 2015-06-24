using System;
using System.Windows;
using FirstFloor.ModernUI.Presentation;
using Syncfusion.Windows.Shared;

namespace XComponent.Common.UI.MessageBox
{
    /// <summary>
    /// Interaction logic for XCMessageBox.xaml
    /// </summary>
    public partial class XCMessageBox : ChromelessWindow
    {        
        private XCMessageBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public static MessageBoxResult ShowMessage(string message, string title, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
        {
            var dlg = new XCMessageBox { Title = title, Message = message, IconType = image };

            var result = MessageBoxResult.None;

            Action<MessageBoxResult> buttonCommandAction = o =>
            {
                result = o;
                dlg.Close();
            };

            //dlg.YesButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.Yes));
            //dlg.NoButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.No));
            //dlg.OkButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.OK));
            //dlg.CancelButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.Cancel));
            //dlg.CloseButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.None));

            //dlg.YesButton.Content = Properties.Resources.Yes;
            //dlg.NoButton.Content = Properties.Resources.No;
            //dlg.OkButton.Content = Properties.Resources.Okay;
            //dlg.CancelButton.Content = Properties.Resources.Cancel;
            //dlg.CloseButton.Content = Properties.Resources.Close;

            dlg.primaryButton.Visibility = Visibility.Visible;
            dlg.secondaryButton.Visibility = Visibility.Visible;
            dlg.tertiaryButton.Visibility = Visibility.Visible;
            switch (messageBoxButton)
            {
                case MessageBoxButton.OK:
                    dlg.primaryButton.Content = Properties.Resources.Okay;
                    dlg.primaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.OK));
                    dlg.secondaryButton.Visibility = Visibility.Collapsed;
                    dlg.tertiaryButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    dlg.primaryButton.Content = Properties.Resources.Okay;
                    dlg.primaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.OK));
                    dlg.secondaryButton.Content = Properties.Resources.Cancel;
                    dlg.secondaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.Cancel));
                    dlg.tertiaryButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    dlg.primaryButton.Content = Properties.Resources.Yes;
                    dlg.primaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.Yes));
                    dlg.secondaryButton.Content = Properties.Resources.No;
                    dlg.secondaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.No));
                    dlg.tertiaryButton.Content = Properties.Resources.Cancel;
                    dlg.tertiaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.Cancel));
                    break;
                case MessageBoxButton.YesNo:
                    dlg.primaryButton.Content = Properties.Resources.Yes;
                    dlg.primaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.Yes));
                    dlg.secondaryButton.Content = Properties.Resources.No;
                    dlg.secondaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.No));
                    dlg.tertiaryButton.Visibility = Visibility.Collapsed;
                    break;
                default:
                    dlg.primaryButton.Content = Properties.Resources.Close;
                    dlg.primaryButton.Command = new RelayCommand(o => buttonCommandAction(MessageBoxResult.None));
                    dlg.secondaryButton.Visibility = Visibility.Collapsed;
                    dlg.tertiaryButton.Visibility = Visibility.Collapsed;
                    break;
            }

            dlg.ShowDialog();
            return result;
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof (string), typeof (XCMessageBox), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty IconTypeProperty = DependencyProperty.Register(
            "IconType", typeof (MessageBoxImage), typeof (XCMessageBox), new PropertyMetadata(default(MessageBoxImage)));

        public MessageBoxImage IconType
        {
            get { return (MessageBoxImage) GetValue(IconTypeProperty); }
            set { SetValue(IconTypeProperty, value); }
        }        
    }
}
