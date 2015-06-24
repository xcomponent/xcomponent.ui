using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace XComponent.Common.UI.Wpf
{
    public static class RibbonHelper
    {
        const string HelpButtonName = "HelpButton";
        const string StackPanelName = "stack";

        static public void HideHelpButton(DependencyObject ribbonWindow)
        {
            Button button = UIHelper.FindChild<Button>(ribbonWindow, HelpButtonName);
            if (button != null)
            {
                button.Visibility = Visibility.Hidden;
            }
        }
        static public StackPanel GetRibbbonMenuStackPanel(DependencyObject ribbonWindow)
        {
            StackPanel stackPanel = UIHelper.FindChild<StackPanel>(ribbonWindow, StackPanelName);
            if (stackPanel != null)
            {
                return stackPanel;
            }

            return null;
        }
        static public void AttachCommandToHelpButton(DependencyObject ribbonWindow, ICommand command)
        {
            Button button = UIHelper.FindChild<Button>(ribbonWindow, HelpButtonName);
            if (button != null)
            {
                button.Command = command;
            }
        }

        //use BackstageControl as generic to avoid referencing syncfusion in Common project
        public static void HideBackstageControl<TRibbonWindowPanel>(DependencyObject ribbonControl, FrameworkElement backStageButton) where TRibbonWindowPanel : FrameworkElement
        {
            DockPanel dock = UIHelper.FindChild<TRibbonWindowPanel>(ribbonControl, "PART_RibbonWindowPanel").Parent as DockPanel;
            if (dock != null)
            {
                Border border = VisualTreeHelper.GetChild(dock, 2) as Border;
                border.Visibility = Visibility.Collapsed;
            }

            backStageButton.Visibility = Visibility.Hidden;
        }
    }
}
