using System.ComponentModel;
using XComponent.Common.UI.Wpf;

namespace XComponent.Common.UI.Grid.ViewModel
{
    public class FilterData : INotifyPropertyChanged
    {
        private object _filterValue;

        private string _filterMappingName;

        private GridFilterBehavior _filterBehavior;

        private GridPredicateType _filterPredicateType;

        private GridFilterType _filterType;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FilterMappingName
        {
            get { return _filterMappingName; }
            set
            {
                _filterMappingName = value;
                this.NotifyPropertyChanged(PropertyChanged, () => FilterMappingName);
            }
        }

        public GridFilterBehavior FilterBehavior
        {
            get { return _filterBehavior; }
            set
            {
                _filterBehavior = value;
                this.NotifyPropertyChanged(PropertyChanged, () => FilterBehavior);
            }
        }

        public GridPredicateType FilterPredicateType
        {
            get { return _filterPredicateType; }
            set
            {
                _filterPredicateType = value;
                this.NotifyPropertyChanged(PropertyChanged, () => FilterPredicateType);
            }
        }

        public GridFilterType FilterType
        {
            get { return _filterType; }
            set
            {
                _filterType = value;
                this.NotifyPropertyChanged(PropertyChanged, () => FilterType);
            }
        }

        public object FilterValue
        {
            get { return _filterValue; }
            set
            {
                _filterValue = value;
                this.NotifyPropertyChanged(PropertyChanged, () => FilterValue);
            }
        }
    }
}
