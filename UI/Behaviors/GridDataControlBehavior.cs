using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Grid;

namespace XComponent.Common.UI.Behaviors
{
    public static class GridDataControlBehavior
    {
        public static readonly DependencyProperty SelectedItemsProperty = 
            DependencyProperty.RegisterAttached("SelectedItems", typeof(ObservableCollection<Object>), typeof(GridDataControlBehavior), new PropertyMetadata(null));

        public static void SetSelectedItems(DependencyObject element, object value)
        {
            if (element is GridDataControl)
            {
                element.SetValue(SelectedItemsProperty, value);
            }

        }

        public static object GetSelectedItems(DependencyObject element)
        {
            return (object)element.GetValue(SelectedItemsProperty);
        }



        public static readonly DependencyProperty EnableSelectedItemBindingProperty =
            DependencyProperty.RegisterAttached("EnableSelectedItemBinding", typeof(bool), typeof(GridDataControlBehavior), new PropertyMetadata(false, OnEnableSelectedItemBinding));

        public static bool GetEnableSelectedItemBinding(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableSelectedItemBindingProperty);
        }

        public static void SetEnableSelectedItemBinding(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableSelectedItemBindingProperty, value);
        }

        private static void OnEnableSelectedItemBinding(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var gdc = obj as GridDataControl;
            if (gdc != null)
            {
                NotifyCollectionChangedEventHandler selectedItemsOnCollectionChanged = (sender, e) => SetSelectedItems(gdc, gdc.SelectedItems);
                if (GetEnableSelectedItemBinding(gdc))
                {
                    gdc.SelectedItems.CollectionChanged += selectedItemsOnCollectionChanged;
                }
                else
                {
                    gdc.SelectedItems.CollectionChanged -= selectedItemsOnCollectionChanged;
                }
            }
        }

        public static readonly DependencyProperty RecordSelectionChangedCommandProperty =
            DependencyProperty.RegisterAttached("RecordSelectionChangedCommand", typeof(ICommand), typeof(GridDataControlBehavior), new PropertyMetadata(null, OnRecordSelectionChangedCommandChangedCallBack));

        public static ICommand GetRecordSelectionChangedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(RecordSelectionChangedCommandProperty);
        }

        public static void SetRecordSelectionChangedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(RecordSelectionChangedCommandProperty, value);
        }

        private static void OnRecordSelectionChangedCommandChangedCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var gdc = obj as GridDataControl;
            if (gdc != null)
            {
                var command = (ICommand) args.NewValue;

                GridDataRecordsSelectionChangedEventHandler selectionChanged = (sender, e) => command.Execute(gdc.SelectedItems);
                if (command != null)
                {
                    gdc.RecordsSelectionChanged += selectionChanged;
                }
                else
                {
                    gdc.RecordsSelectionChanged -= selectionChanged;
                }
            }
        }
    }
}
