using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Monads;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Syncfusion.Data.Extensions;
using XComponent.Common.UI.Converter;
using XComponent.Common.UI.Wpf;

namespace XComponent.Common.UI.Grid.ViewModel
{
    public class ColumnsVisualizationViewModel : DependencyObject
    {
        public static readonly DependencyProperty ColumnsToExcludeProperty = DependencyProperty.Register("ColumnsToExclude", typeof(ObservableCollection<string>), typeof(ColumnsVisualizationViewModel));
        public static readonly DependencyProperty AllColumnsVisibleProperty = DependencyProperty.Register("AllColumnsVisible", typeof(bool?), typeof(ColumnsVisualizationViewModel));
        

        private readonly Dictionary<string, ColumnViewModel> columnsViewModel = new Dictionary<string, ColumnViewModel>();



        public ObservableCollection<string> ColumnsToExclude
        {
            get { return (ObservableCollection<string>)GetValue(ColumnsToExcludeProperty); }
            set { SetValue(ColumnsToExcludeProperty, value); }
        }

        public ReadOnlyDictionary<string, ColumnViewModel> ColumnsViewModel { get { return new ReadOnlyDictionary<string,ColumnViewModel>(columnsViewModel); } }


        public IEnumerable<ColumnViewModel> AllColumnsViewModel { get { return ColumnsViewModel.Values; } }


        public bool? AllColumnsVisible
        {
            get { return (bool?)GetValue(AllColumnsVisibleProperty); }
            set { SetValue(AllColumnsVisibleProperty, value); }
        }

        public ColumnsVisualizationData ToXmlSerializableData()
        {
            return new ColumnsVisualizationData() { ColumnsData = ColumnsViewModel.Select(x => x.Value).OrderBy(x => x.Position).Select(x => x.ToXmlSerializableData()).ToList() };
        }

        public void FromXmlSerializableData(ColumnsVisualizationData data)
        {
            foreach (ColumnData columnData in data.ColumnsData)
            {
                AddColumn(columnData.DisplayName, new ColumnViewModel(columnData));
            }
        }

        internal void AddColumn(string key, ColumnViewModel viewmodel)
        {
            columnsViewModel[key] = viewmodel;
            MultiBinding multibinding = new MultiBinding();
            multibinding.Mode = BindingMode.TwoWay;
            multibinding.Converter = new BoolAggregationConverter();
            NotBooleanConverter notconverter = new NotBooleanConverter();
            foreach (ColumnViewModel column in AllColumnsViewModel)
            {
                Binding binding = new Binding();
                binding.Source = column;
                binding.Mode = BindingMode.TwoWay;
                binding.Path = new PropertyPath(WPFExtensions.PropertyName(() => column.IsHidden));
                binding.Converter = notconverter;
                multibinding.Bindings.Add(binding);
            }
            BindingOperations.SetBinding(this, AllColumnsVisibleProperty, multibinding);
        }

        // edtails key = display name / value = mapping name
        public void UpdateDisplayNames(Dictionary<string, string> details)
        {
            // key = old display name / value = new display name
            var displayNameToUpdateDictionary = new Dictionary<string, string>();

            // detection wrong display name
            columnsViewModel.ForEach(e =>
            {
                if (!details.ContainsKey(e.Key))
                {
                    details.Where(el => el.Value == e.Value.MappingName).ForEach(t => displayNameToUpdateDictionary.Add(e.Key, t.Key));
                }
            });

            // updating display names in columnsViewModel dictionary
            displayNameToUpdateDictionary.ForEach(e =>
            {
                var viewModel = columnsViewModel[e.Key];
                columnsViewModel.Remove(e.Key);
                viewModel.DisplayName = e.Value;
                AddColumn(e.Value, viewModel);
            });
        }
    }
}
