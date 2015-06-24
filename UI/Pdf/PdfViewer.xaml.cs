using System;
using System.Monads;
using System.Windows;
using System.Windows.Controls;

namespace XComponent.Common.UI.Pdf
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PdfViewer : UserControl, IDisposable
    {
        public PdfViewer()
        {
            InitializeComponent();
            winFormPdfHost = pdfHost.Child as WinFormPdfHost;
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof (string), typeof (PdfViewer), new PropertyMetadata(default(string), SourcePropertyChangedCallback));

        private static void SourcePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewer = dependencyObject as PdfViewer;
            viewer.Do(e => e.LoadFile(dependencyPropertyChangedEventArgs.NewValue as string));
        }

        public string Source
        {
            get { return (string) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }        

        public bool ShowToolBar
        {
            get { return showToolBar; }
            set
            {
                showToolBar = value;
                winFormPdfHost.SetShowToolBar(showToolBar);
            }
        }


        private void LoadFile(string path)
        {
            winFormPdfHost.LoadFile(path);
        }
        
        private bool showToolBar;
        private readonly WinFormPdfHost winFormPdfHost;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.winFormPdfHost.Do(e => e.Dispose());
            }
        }
    }
}
