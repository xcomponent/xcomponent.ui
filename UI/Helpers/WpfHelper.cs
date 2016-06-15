using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace XComponent.Common.UI.Helpers
{
    public static class WpfHelper
    {
        public static T FindAncestor<T>(this Visual child)
            where T : Visual
        {

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !typeof(T).IsInstanceOfType(parent))
            {

                parent = VisualTreeHelper.GetParent(parent);

            }

            return (parent as T);

        }

        public static Visual FindAncestor(this Visual child, Type typeAncestor)
        {

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !typeAncestor.IsInstanceOfType(parent))
            {

                parent = VisualTreeHelper.GetParent(parent);

            }

            return (parent as Visual);

        }

        public static Visual FindAncestor(this Visual child, int level)
        {
            if(level<0)
            {
                return child;
            }
            DependencyObject parent = child;
            while (level != 0 && parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                level--;
            }
            return parent as Visual;

        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }

        public static void NotifyPropertyChanged<TProperty>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, Expression<Func<TProperty>> property)
        {
            if (handler != null)
            {
                try
                {
                    string propertyName = PropertyName(property);
                    if (propertyName != null)
                    {
                        handler(sender, new PropertyChangedEventArgs(propertyName));
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static string PropertyName<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member.Name;
        }
    }
}
