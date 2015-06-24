using System.Collections.Specialized;
using System.Windows.Interactivity;
using Syncfusion.Windows.Controls.Input;

namespace XComponent.Common.UI.Suggest
{
    public class AsyncSuggestBehavior : Behavior<SfTextBoxExt>
    {
        private bool initialized;

        protected override void OnAttached()
        {
            this.AssociatedObject.Filter = (search, item) => true;
            this.AssociatedObject.Loaded += (sender2, args2) =>
            {
                var source = this.AssociatedObject.AutoCompleteSource as INotifyCollectionChanged;
                if (source != null && ! this.initialized)
                {
                    source.CollectionChanged += SourceOnCollectionChanged;
                    this.initialized = true;
                }
            };
            base.OnAttached();
        }

        private void SourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {                        
            this.AssociatedObject.FilterSuggestions();            
        }

        protected override void OnDetaching()
        {
            var source = this.AssociatedObject.AutoCompleteSource as INotifyCollectionChanged;
            if (source != null && this.initialized)
            {
                source.CollectionChanged -= SourceOnCollectionChanged;
                this.initialized = false;
                this.AssociatedObject.Filter = null;
            }
            base.OnDetaching();
        }
    }
}
