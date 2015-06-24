using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using Syncfusion.Windows.Controls.Grid;
using XComponent.Common.UI.Grid.Converter;
using XComponent.Common.UI.Grid.ViewModel;
using XComponent.Common.UI.Wpf;

namespace XComponent.Common.UI.Grid.Behavior
{
    public class ColumnAppearanceBehavior : Behavior<GridDataControl>
    {
        private static readonly DependencyProperty ColumnsVisualizationProperty = DependencyProperty.Register("ColumnsVisualization", typeof(ColumnsVisualizationViewModel), typeof(ColumnAppearanceBehavior));
        private static readonly DependencyProperty IsDetailsProperty = DependencyProperty.Register("IsDetails", typeof(bool), typeof(ColumnAppearanceBehavior));
        private static readonly DependencyProperty RelationalColumnProperty = DependencyProperty.Register("RelationalColumn", typeof(string), typeof(ColumnAppearanceBehavior));

        private readonly ObservableCollection<string> columnsToExclude = new ObservableCollection<string>();
        private GridDataVisibleColumns _gridVisibleColumns;

        public ObservableCollection<string> ColumnsToExclude
        {
            get { return columnsToExclude; }
        }

        public ColumnsVisualizationViewModel ColumnsVisualization
        {
            get { return (ColumnsVisualizationViewModel)GetValue(ColumnsVisualizationProperty); }
            set { SetValue(ColumnsVisualizationProperty, value); }
        }

        public bool IsDetails
        {
            get { return (bool)GetValue(IsDetailsProperty); }
            set { SetValue(IsDetailsProperty, value); }
        }

        public string RelationalColumn
        {
            get { return (string)GetValue(RelationalColumnProperty); }
            set { SetValue(RelationalColumnProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private GridDataVisibleColumns GetVisibleColumns()
        {
            return IsDetails
                ? AssociatedObject.Relations.FirstOrDefault(e => e.RelationalColumn == RelationalColumn).TableProperties.VisibleColumns
                : AssociatedObject.VisibleColumns;

        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _gridVisibleColumns = GetVisibleColumns();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (ColumnsVisualization == null)
                return;

            Binding exclusionBinding = new Binding();
            exclusionBinding.Source = this;
            exclusionBinding.Path = new PropertyPath(WPFExtensions.PropertyName(() => ColumnsToExclude));
            exclusionBinding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(ColumnsVisualization, ColumnsVisualizationViewModel.ColumnsToExcludeProperty, exclusionBinding);


            ColumnLengthConverter columnLengthConverter = new ColumnLengthConverter();
            Binding binding;
            int lastposition = ColumnsVisualization.ColumnsViewModel.Count;
            GridDataVisibleColumn[] orderedColumns = new GridDataVisibleColumn[_gridVisibleColumns.Count];

            foreach (GridDataVisibleColumn column in _gridVisibleColumns)
            {
                ColumnViewModel viewmodel;

                if (!ColumnsVisualization.ColumnsViewModel.TryGetValue(column.HeaderText, out viewmodel))
                {
                    viewmodel = new ColumnViewModel(column.MappingName, column.HeaderText);
                    ColumnsVisualization.AddColumn(column.HeaderText, viewmodel);
                    viewmodel.Width = column.ActualWidth;
                    viewmodel.IsHidden = column.IsHidden;
                    viewmodel.Position = lastposition;
                    orderedColumns[lastposition++] = column;
                }
                else
                {
                    orderedColumns[viewmodel.Position] = column;
                }
                binding = new Binding();
                binding.Source = viewmodel;
                binding.Path = new PropertyPath(WPFExtensions.PropertyName(() => viewmodel.Width));
                binding.Converter = columnLengthConverter;
                binding.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(column, GridDataVisibleColumn.WidthProperty, binding);

                if (!ColumnsToExclude.Contains(column.MappingName))
                {
                    binding = new Binding();
                    binding.Source = viewmodel;
                    binding.Path = new PropertyPath(WPFExtensions.PropertyName(() => viewmodel.IsHidden));
                    binding.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(column, GridDataVisibleColumn.IsHiddenProperty, binding);
                }
            }
            _gridVisibleColumns.Clear();
            foreach (GridDataVisibleColumn column in orderedColumns)
            {
                _gridVisibleColumns.Add(column);
            }

            ((INotifyCollectionChanged)_gridVisibleColumns).CollectionChanged += ColumnAppearanceBehavior_CollectionChanged;
        }

        void ColumnAppearanceBehavior_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = 0; i < _gridVisibleColumns.Count; i++)
                {
                    var column = _gridVisibleColumns[i];
                    ColumnsVisualization.ColumnsViewModel[column.HeaderText].Position = i;
                }
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            ((INotifyCollectionChanged)_gridVisibleColumns).CollectionChanged -= ColumnAppearanceBehavior_CollectionChanged;
        }
    }
}
