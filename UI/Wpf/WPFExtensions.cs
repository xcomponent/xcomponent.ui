using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace XComponent.Common.UI.Wpf
{
    public static class WPFExtensions
    {

        static public string PropertyName<TProperty>(Expression<Func<TProperty>> property)
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

        static public void NotifyPropertyChanged<TProperty>(this INotifyPropertyChanged sender,
        PropertyChangedEventHandler handler,Expression<Func<TProperty>> property)
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
                catch (Exception)
                {

                }
            }
        }
    }
}
