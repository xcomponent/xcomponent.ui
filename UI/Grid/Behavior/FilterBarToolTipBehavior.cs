using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using Syncfusion.Windows.Controls.Grid;

namespace XComponent.Common.UI.Grid.Behavior
{
    public class FilterBarToolTipBehavior : Behavior<GridDataControl>
    {
        public static readonly DependencyProperty ToolTipTemplateProperty = DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(FilterBarToolTipBehavior));
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(ToolTip), typeof(FilterBarToolTipBehavior));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ModelLoaded += ModelLoaded;
        }

        private void ModelLoaded(object sender, EventArgs e)
        {
            this.AssociatedObject.Model.QueryCellInfo += Model_QueryCellInfo;
        }

        void Model_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (e.Style.CellType == "FilterBarCell")
            {
                e.Style.ShowTooltip = true;
                e.Style.TooltipTemplate = ToolTipTemplate;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Model.QueryCellInfo -= Model_QueryCellInfo;
            AssociatedObject.ModelLoaded -= ModelLoaded;
        }

        public DataTemplate ToolTipTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
            set { SetValue(ToolTipTemplateProperty, value); }
        }

        public ToolTip ToolTip
        {
            get { return (ToolTip)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }
    }
}
