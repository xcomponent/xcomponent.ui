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
        private bool _isBehaviorHooked;

        protected override void OnAttached()
        {
            AssociatedObject.ModelLoaded += AssociatedObjectOnModelLoaded;

            base.OnAttached();
        }

        private void AssociatedObjectOnModelLoaded(object sender, EventArgs eventArgs)
        {
            if (!_isBehaviorHooked && !AssociatedObject.Model.IsInitialized)
            {
                AssociatedObject.Model.Initialized += ModelOnInitialized;
            }
            else if (!_isBehaviorHooked)
            {
                HookBehavior();
            }
        }

        private void ModelOnInitialized(object sender, EventArgs eventArgs)
        {
            HookBehavior();
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

        private void HookBehavior()
        {
            SubscribeToCollectionChanged();

            // We need to re-subscribe to the event in case the source collection changed..
            AssociatedObject.ItemsSourceChanged += OnItemsSourceChanged;

            _isBehaviorHooked = true;
        }

        protected override void OnDetaching()
        {
            UnSubscribeFromCollectionChanged();
            AssociatedObject.ItemsSourceChanged -= OnItemsSourceChanged;
            AssociatedObject.Model.Initialized -= ModelOnInitialized;
            AssociatedObject.ModelLoaded -= AssociatedObjectOnModelLoaded;

            base.OnDetaching();
        }
    }
}
