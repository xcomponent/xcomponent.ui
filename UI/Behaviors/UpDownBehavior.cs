using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Syncfusion.Windows.Shared;

namespace XComponent.Common.UI.Behaviors
{
    public class UpDownBehavior
    {
        public static readonly DependencyProperty StepChangedCommandProperty = DependencyProperty.RegisterAttached("StepChangedCommand",
            typeof(ICommand), typeof(UpDownBehavior), new PropertyMetadata(StepChangedCallBack));

        public static readonly DependencyProperty ValueChangedCommandProperty = DependencyProperty.RegisterAttached("ValueChangedCommand",
            typeof(ICommand), typeof(UpDownBehavior), new PropertyMetadata(ValueChangedCallBack));

        private static void StepChangedCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var upDown = obj as UpDown;
            if (upDown == null)
            {
                return;
            }

            if (args.NewValue != null)
            {
                upDown.StepChanged += UpDownOnStepChanged;
            }
            else
            {
                upDown.StepChanged -= UpDownOnStepChanged;                
            }
        }

        private static void ValueChangedCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var upDown = obj as UpDown;
            if (upDown == null)
            {
                return;
            }

            if (args.NewValue != null)
            {
                upDown.ValueChanged += UpDownOnValueChanged;
            }
            else
            {
                upDown.ValueChanged -= UpDownOnValueChanged;                
            }
        }

        private static void UpDownOnStepChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var upDown = dependencyObject as UpDown;
            if (upDown != null)
            {
                var command = GetStepChangedCommand(upDown);

                if (command != null)
                {
                    if (command.CanExecute(e))
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => command.Execute(e.NewValue)));
                    }
                }
            }
        }

        private static void UpDownOnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var upDown = dependencyObject as UpDown;
            if (upDown != null)
            {
                var command = GetValueChangedCommand(upDown);

                if (command != null)
                {
                    if (command.CanExecute(e))
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke((Action) (() =>
                        {
                            var parameters = new ValueChangedParameters((double)e.OldValue, (double) e.NewValue);
                            command.Execute(parameters);
                        }));
                    }
                }
            }
        }

        public static void SetStepChangedCommand(UIElement obj, ICommand value)
        {
            obj.SetValue(StepChangedCommandProperty, value);
        }

        public static ICommand GetStepChangedCommand(UIElement obj)
        {
            return (ICommand)obj.GetValue(StepChangedCommandProperty);
        }


        public static void SetValueChangedCommand(UIElement obj, ICommand value)
        {
            obj.SetValue(ValueChangedCommandProperty, value);
        }

        public static ICommand GetValueChangedCommand(UIElement obj)
        {
            return (ICommand)obj.GetValue(ValueChangedCommandProperty);
        }

    }
}
