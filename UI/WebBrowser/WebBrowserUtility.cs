using System;
using System.Web;
using System.Windows;

namespace XComponent.Common.UI.WebBrowser
{
    public static class WebBrowserUtility
    {        
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var browser = o as System.Windows.Controls.WebBrowser;
            if (browser != null)
            {
                var uri = e.NewValue as string;
                browser.Navigate(!String.IsNullOrEmpty(uri) ? new Uri(HttpUtility.UrlPathEncode(uri)) : null);                
            }
        }

    }
}
