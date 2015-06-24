using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace XComponent.Common.UI.Docking
{
    public class DefaultDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            
            if (element != null && item != null)
            {
                return element.Resources.Values.OfType<DataTemplate>().ToList().FirstOrDefault(value =>
                {
                    var templateType = value.DataType as Type;
                    return templateType != null && templateType == item.GetType();
                });                
            }
            return null;
        }
    }
}
