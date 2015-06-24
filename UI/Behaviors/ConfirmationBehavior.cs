using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using XComponent.Common.UI.MessageBox;

namespace XComponent.Common.UI.Behaviors
{
    public class ConfirmationBehaviorBase<T> : Behavior<T> where T : ButtonBase
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ConfirmationBehaviorBase<T>));
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(ConfirmationBehaviorBase<T>));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ConfirmationBehaviorBase<T>));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ConfirmationBehaviorBase<T>));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private bool isAttached;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnButtonClick;
            isAttached = true;
            CommandCanExecuteChanged(Command, EventArgs.Empty);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= OnButtonClick;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (Command == null || !Command.CanExecute(CommandParameter))
                return;

            if (ShouldConfirm())
            {
                MessageBoxResult result = MessageService.Current.ShowMessage(Message, Caption, MessageBoxButton.YesNo,
                                                          MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    OnConfirmed();
                else
                    OnNotConfirmed();
            }
            else
            {
                OnConfirmed();
            }
        }

        protected virtual bool ShouldConfirm()
        {
            return true;
        }

        protected virtual void OnConfirmed()
        {
            Command.Execute(CommandParameter);
        }

        protected virtual void OnNotConfirmed() { }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == CommandProperty)
            {
                ICommand command;
                if ((command = e.OldValue as ICommand) != null)
                    command.CanExecuteChanged -= CommandCanExecuteChanged;
                if ((command = e.NewValue as ICommand) != null)
                    command.CanExecuteChanged += CommandCanExecuteChanged;
                if (isAttached)
                    CommandCanExecuteChanged(Command, EventArgs.Empty);
            }
            if (e.Property == CommandParameterProperty)
            {
                if (isAttached)
                    CommandCanExecuteChanged(Command, EventArgs.Empty);
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                AssociatedObject.IsEnabled = Command != null && Command.CanExecute(CommandParameter);
            }
        }

    }

    public class ConfirmationBehavior : ConfirmationBehaviorBase<ButtonBase>
    {
    }

    public class ToggleConfirmationBehavior : ConfirmationBehaviorBase<ToggleButton>
    {
        protected override bool ShouldConfirm()
        {
            return AssociatedObject.IsChecked == true;
        }

        protected override void OnNotConfirmed()
        {
            AssociatedObject.IsChecked = false;
        }
    }
}
