using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using XComponent.Common.UI.Helpers;

namespace XComponent.Common.UI.Loading
{
    public class LoadingBehavior : Behavior<UIElement>
    {
        public LoadingBehavior()
        {
            this.Delay = 0;
        }

        protected override void OnAttached()
        {            
            this.AssociatedObject.Visibility = Visibility.Hidden;            
            
            base.OnAttached();
        }

        private async void Process(LoadingStatusEnum status)
        {

            if (status != LoadingStatusEnum.Loading)
            {
                var delayValue = this.Delay;
                await Task.Run(() => Thread.Sleep(delayValue));
            }

            var adornerControl = WpfHelper.FindAncestor(this.AssociatedObject, typeof (LoadingAdornedControl)) as LoadingAdornedControl;
            if (adornerControl == null)
            {
                if (ThrowException)
                {
                    throw new ArgumentException("the control is not inside a LoadingAdornedControl");                    
                }
                else
                {
                    return;
                }
            }

            switch (status)
            {                
                case LoadingStatusEnum.Loaded:
                    adornerControl.IsAdornerVisible = false;
                    this.AssociatedObject.Visibility = Visibility.Visible;            
                    break;
                case LoadingStatusEnum.Timeout:
                    adornerControl.ShowTimeoutMessage();
                    break;
                case LoadingStatusEnum.Loading:
                    adornerControl.IsAdornerVisible = true;
                    this.AssociatedObject.Visibility = Visibility.Hidden;    
                    break;    
            }            
        }

        public static readonly DependencyProperty ThrowExceptionProperty = DependencyProperty.Register(
          "ThrowException", typeof(bool), typeof(LoadingBehavior), new PropertyMetadata(true));


        public static readonly DependencyProperty LoadingStatusProperty = DependencyProperty.Register(
            "LoadingStatus", typeof(LoadingStatusEnum), typeof(LoadingBehavior), new PropertyMetadata(LoadingStatusEnum.Loading, IsLoadingCompletedPropertyChangedCallback));

        private static void IsLoadingCompletedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as LoadingBehavior;
            behavior.Process((LoadingStatusEnum)dependencyPropertyChangedEventArgs.NewValue);                
        }

        public LoadingStatusEnum LoadingStatus
        {
            get { return (LoadingStatusEnum)GetValue(LoadingStatusProperty); }
            set { SetValue(LoadingStatusProperty, value); }
        }

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay", typeof (int), typeof (LoadingBehavior), new PropertyMetadata(default(int)));

        public int Delay
        {
            get { return (int) GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public bool ThrowException
        {
            get { return (bool)GetValue(ThrowExceptionProperty); }
            set { SetValue(ThrowExceptionProperty, value); }
        }
    }
}
