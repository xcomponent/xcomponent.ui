using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Grid;

namespace XComponent.Common.UI.Grid.Behavior
{
    public class AutoScrollOnNewRowBehavior : Behavior<GridDataControl>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;

            base.OnAttached();
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SubscribeToCollectionChanged();

            // We need to re-subscribe to the event in case the source collection changed..
            AssociatedObject.ItemsSourceChanged += OnItemsSourceChanged;
        }

        private void ViewOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                AssociatedObject.Model.View.Refresh();

                foreach (var gridControl in AssociatedObject.Model.Views)
                {
                    gridControl.ScrollToBottom();
                }

                // if no element is selected then select the newly added one..
                if (AssociatedObject.SelectedItem == null
                    && notifyCollectionChangedEventArgs.NewItems != null
                    && notifyCollectionChangedEventArgs.NewItems.Count != 0)
                {
                    AssociatedObject.SelectedItem = notifyCollectionChangedEventArgs.NewItems[0];
                }
            }
        }

        private void OnItemsSourceChanged(object sender, SyncfusionRoutedEventArgs args)
        {
            SubscribeToCollectionChanged();
        }

        private void SubscribeToCollectionChanged()
        {
            AssociatedObject.Model.View.CollectionChanged += ViewOnCollectionChanged;
        }

        private void UnSubscribeFromCollectionChanged()
        {
            AssociatedObject.Model.View.CollectionChanged -= ViewOnCollectionChanged;
        }

        protected override void OnDetaching()
        {
            UnSubscribeFromCollectionChanged();
            AssociatedObject.ItemsSourceChanged -= OnItemsSourceChanged;
            AssociatedObject.Loaded -= AssociatedObjectOnLoaded;

            base.OnDetaching();
        }
    }
}
