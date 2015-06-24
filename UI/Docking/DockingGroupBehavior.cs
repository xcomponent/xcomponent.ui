using System;
using System.Windows;
using System.Windows.Interactivity;
using Syncfusion.Windows.Tools.Controls;

namespace XComponent.Common.UI.Docking
{
    public class DockingGroupBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            if (string.IsNullOrWhiteSpace(this.GroupName)) throw new ArgumentException("Missing argument", "GroupName");

            if (this.DockSide == DockSide.Tabbed || this.DockSide == DockSide.None) throw new ArgumentException("Invalid value", "DockSide");

            var groupElement = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, this.GroupName);

            if (groupElement == null)
            {
                this.AssociatedObject.Name = this.GroupName;
                DockingManager.SetState(this.AssociatedObject, DockState.Dock);
                DockingManager.SetSideInDockedMode(this.AssociatedObject, this.DockSide);
            }
            else
            {
                DockingManager.SetSideInDockedMode(this.AssociatedObject, DockSide.Tabbed);
                DockingManager.SetTargetNameInDockedMode(this.AssociatedObject, this.GroupName);
            }

            base.OnAttached();
        }        

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName", typeof (string), typeof (DockingGroupBehavior), new PropertyMetadata(default(string)));

        public string GroupName
        {
            get { return (string) GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty DockSideProperty = DependencyProperty.Register(
            "DockSide", typeof (DockSide), typeof (DockingGroupBehavior), new PropertyMetadata(default(DockSide)));

        public DockSide DockSide
        {
            get { return (DockSide) GetValue(DockSideProperty); }
            set { SetValue(DockSideProperty, value); }
        }
    }
}
