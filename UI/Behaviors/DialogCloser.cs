using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;

namespace XComponent.Common.UI.Behaviors
{
    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogCloser),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var window = d as ModernDialog;
            if (window != null)
                window.DialogResult = e.NewValue as bool?;
        }
        public static void SetDialogResult(ModernDialog target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }
    }
}
