using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XComponent.Common.UI.Grid.ViewModel
{
    public class ColumnViewModel : DependencyObject
    {
        private static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ColumnViewModel));
        private static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(ColumnViewModel));
        
        private readonly string mappingName;
        private readonly string displayName;

        static ColumnViewModel()
        {
            AutoMapper.Mapper.CreateMap<ColumnViewModel, ColumnData>();
        }


        public ColumnViewModel(string mappingName, string displayName)
        {
            this.mappingName = mappingName;
            this.displayName = displayName;
        }

        public ColumnViewModel(ColumnData columnData) : this(columnData.MappingName, columnData.DisplayName)
        {
            this.Width = columnData.Width;
            this.IsHidden = columnData.IsHidden;
            this.Position = columnData.Position;
        }

        public string MappingName { get { return this.mappingName; } }

        public string DisplayName { get { return this.displayName; } }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public bool IsHidden
        {
            get { return (bool)GetValue(IsHiddenProperty); }
            set { SetValue(IsHiddenProperty, value); }
        }

        public int Position { get; set; }

        internal ColumnData ToXmlSerializableData()
        {
            return AutoMapper.Mapper.Map<ColumnData>(this);
        }
    }
}
