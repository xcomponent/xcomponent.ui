using System.Windows.Controls;
using System.Windows.Interactivity;

namespace XComponent.Common.UI.Behaviors
{
    public class SelectOnFocusBehavior : Behavior<TextBox>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.GotKeyboardFocus += AssociatedObject_GotFocus;            
        }

        void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.SelectAll();
        }        

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
        }
    }
}
