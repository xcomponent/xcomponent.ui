using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Syncfusion.Linq;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Data;
using XComponent.Common.UI.Grid.ViewModel;

namespace XComponent.Common.UI.Grid.Behavior
{
    public class FiltersProviderBehavior : Behavior<GridDataControl>
    {
        public static DependencyProperty FilterChangedCommandProperty = DependencyProperty.Register("FilterChangedCommand", typeof(ICommand), typeof(FiltersProviderBehavior), new UIPropertyMetadata(null));

        public static DependencyProperty InitialFiltersProperty = DependencyProperty.Register("InitialFilters", typeof(ObservableCollection<FilterData>), typeof(FiltersProviderBehavior), new UIPropertyMetadata(null));

        public ICommand FilterChangedCommand
        {
            get { return (ICommand)GetValue(FilterChangedCommandProperty); }
            set { SetValue(FilterChangedCommandProperty, value); }
        }

        public ObservableCollection<FilterData> InitialFilters
        {
            get { return (ObservableCollection<FilterData>)GetValue(InitialFiltersProperty); }
            set { SetValue(InitialFiltersProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Model.FilterChanged += HandleFilterChanged;
            AssociatedObject.Loaded += HandleAssociatedObjectLoaded;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Model.FilterChanged -= HandleFilterChanged;
            AssociatedObject.Loaded -= HandleAssociatedObjectLoaded;

            base.OnDetaching();
        }

        private void HandleAssociatedObjectLoaded(object sender, RoutedEventArgs arg)
        {
            if (InitialFilters != null)
            {
                foreach (var filterData in InitialFilters)
                {
                    AssociatedObject.VisibleColumns[filterData.FilterMappingName].Filters.Add(new FilterPredicate()
                    {
                        FilterBehavior = (FilterBehavior)filterData.FilterBehavior,
                        PredicateType = (PredicateType)filterData.FilterPredicateType,
                        FilterType = (FilterType)filterData.FilterType,
                        FilterValue = filterData.FilterValue
                    });
                }

                AssociatedObject.Model.View.RefreshFilters();
            }
        }

        private void HandleFilterChanged(object obj, GridFilterEventArgs arg)
        {
            if (FilterChangedCommand.CanExecute(AssociatedObject.DataContext))
            {
                FilterChangedCommand.Execute(
                        new ColumnFilterData()
                        {
                            ColumnMappingName = arg.Column.MappingName,
                            ColumnFilters = (arg.Column.Filters != null) ? arg.Column.Filters.Select(f =>
                                new FilterData()
                                {
                                    FilterMappingName = arg.Column.MappingName,
                                    FilterBehavior = (GridFilterBehavior)f.FilterBehavior,
                                    FilterPredicateType = (GridPredicateType)f.PredicateType,
                                    FilterType = (GridFilterType)f.FilterType,
                                    FilterValue = f.FilterValue
                                }).ToList()
                                :
                                new List<FilterData>()
                        });
            }
        }
    }
}
