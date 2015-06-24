using System;
using System.Monads;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Input;

namespace XComponent.Common.UI.Suggest
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:XComponent.Gui.Common.Suggest"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:XComponent.Gui.Common.Suggest;assembly=XComponent.Gui.Common.Suggest"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:AutoCompleteSearchTextBox/>
    ///
    /// </summary>
    public class AutoCompleteSearchTextBox : SfTextBoxExt, ICommandSource
    {
        public AutoCompleteSearchTextBox()
        {            
            this.AllowPointerEvents = true;
            this.IsTextEmpty = true;
            this.DefaultStyleKey = typeof(AutoCompleteSearchTextBox);
            this.TextChanged += (sender, args) =>
            {
                var popupControl = this.GetTemplateChild("PART_Popup") as Popup;
                if (!this.isSuggestFocusLost)
                {
                    this.QueryValue = this.Text;
                    popupControl.Do(e => e.IsOpen = true);
                }
                else
                {
                    popupControl.Do(e => e.IsOpen = false);
                }
                this.isSuggestFocusLost = false;
                this.IsTextEmpty = string.IsNullOrWhiteSpace(this.Text);
            };
            this.LostKeyboardFocus += (sender, args) => Dispatcher.BeginInvoke((Action)(() =>
            {
                this.isSuggestFocusLost = true;
                this.Text = string.Empty;
                var watermarkControl = this.GetTemplateChild("PART_Watermark") as FrameworkElement;
                watermarkControl.Do(e => e.Visibility = Visibility.Visible);
                this.isPopupClicked = false;
            }));

        }


        private bool isSuggestFocusLost;
        private bool isPopupClicked;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);   
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {                
                this.isPopupClicked = false;
                this.isSuggestFocusLost = true;
                this.QueryValue = this.Text;
                this.Text = string.Empty;
                var popupControl = this.GetTemplateChild("PART_Popup") as Popup;
                popupControl.Do(t => t.IsOpen = false);
                var watermarkControl = this.GetTemplateChild("PART_Watermark") as FrameworkElement;
                watermarkControl.Do(t => t.Visibility = Visibility.Visible);
                this.InvokeCommand();
            }                     
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewGotKeyboardFocus(e);        
            var watermarkControl = this.GetTemplateChild("PART_Watermark") as FrameworkElement;
            watermarkControl.Do(t => t.Visibility = Visibility.Collapsed);            
        }

        public override void OnApplyTemplate()
        {            
            var searchIcon = this.GetTemplateChild("PART_Logo_Search") as FrameworkElement;            
            searchIcon.Do(e => WeakEventManager<FrameworkElement, MouseButtonEventArgs>
                .AddHandler(e, "MouseLeftButtonDown", OnSearchClick));

            var cancelIcon = this.GetTemplateChild("PART_Logo_Cancel") as FrameworkElement;
            cancelIcon.Do(e => WeakEventManager<FrameworkElement, MouseButtonEventArgs>
                .AddHandler(e, "MouseLeftButtonDown", OnClearSearch));

            var suggestionBox = this.GetTemplateChild("PART_SuggestionBox") as Selector;           
            suggestionBox.Do(e => WeakEventManager<Selector, SelectionChangedEventArgs>
                .AddHandler(e, "SelectionChanged", SuggestionBoxOnSelectionChanged));
            suggestionBox.Do(e => WeakEventManager<Selector, MouseButtonEventArgs>
                .AddHandler(e, "PreviewMouseDown", OnSuggestionBoxClick));

            var watermarkControl = this.GetTemplateChild("PART_Watermark") as FrameworkElement;
            watermarkControl.Do(e => e.IsVisibleChanged += OnWatermarkVisibleChanged);

            base.OnApplyTemplate();
        }

        private void OnWatermarkVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!((bool) dependencyPropertyChangedEventArgs.NewValue) && ((bool) dependencyPropertyChangedEventArgs.OldValue))
            {
                this.QueryValue = this.Text;
                this.isSuggestFocusLost = false;
            }
        }

        private void OnSuggestionBoxClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            this.isPopupClicked = true;
        }

        private void SuggestionBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (this.isPopupClicked)
            {
                var suggestionBox = this.GetTemplateChild("PART_SuggestionBox") as Selector;
                suggestionBox.Do(e =>
                {
                    string newValue;
                    if (!string.IsNullOrWhiteSpace(SearchItemPath) && e.SelectedValue != null)
                    {
                        var pinfo = e.SelectedValue.GetType().GetProperty(SearchItemPath);
                        if (pinfo != null)
                        {
                            newValue = pinfo.GetValue(e.SelectedValue) as string;
                        }
                        else
                        {
                            newValue = null;
                        }
                    }
                    else
                    {
                        newValue = e.SelectedValue as string;
                    }
                    if (newValue != null)
                    {
                        this.Text = newValue;
                        this.isPopupClicked = false;
                        this.isSuggestFocusLost = true;
                        this.QueryValue = this.Text;
                        this.Text = string.Empty;
                        var watermarkControl = this.GetTemplateChild("PART_Watermark") as FrameworkElement;
                        watermarkControl.Do(t => t.Visibility = Visibility.Visible);
                        this.InvokeCommand();
                    }
                });
            }
        }      

        private void OnClearSearch(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            this.Text = string.Empty;
        }

        private void OnSearchClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var watermarkControl = this.GetTemplateChild("PART_Watermark") as FrameworkElement;
            watermarkControl.Do(e => e.Visibility = Visibility.Collapsed);
        }

        static AutoCompleteSearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteSearchTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteSearchTextBox)));
        }

        public static readonly DependencyProperty IsTextEmptyProperty = DependencyProperty.Register(
            "IsTextEmpty", typeof (bool), typeof (AutoCompleteSearchTextBox), new PropertyMetadata(default(bool)));

        public bool IsTextEmpty
        {
            get { return (bool) GetValue(IsTextEmptyProperty); }
            set { SetValue(IsTextEmptyProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof (ICommand), typeof (AutoCompleteSearchTextBox), new PropertyMetadata(default(ICommand), OnCommandChanged));

        private static void OnCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as AutoCompleteSearchTextBox;
            control.Do(c => c.HookUpCommand(dependencyPropertyChangedEventArgs.OldValue as ICommand, dependencyPropertyChangedEventArgs.NewValue as ICommand));
        }

        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= Command_CanExecuteChanged;
            }
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += Command_CanExecuteChanged;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            if (Command != null)
            {
                var command = Command as RoutedCommand;
                // RoutedCommand.
                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        IsEnabled = true;
                    }
                    else
                    {
                        IsEnabled = false;
                    }
                }
                // Not a RoutedCommand.
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        IsEnabled = true;
                    }
                    else
                    {
                        IsEnabled = false;
                    }
                }
            }
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof (object), typeof (AutoCompleteSearchTextBox), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public IInputElement CommandTarget { get; set; }

        private void InvokeCommand()
        {
            if (Command != null && this.IsInitialized && Command.CanExecute(CommandParameter))
            {                
                var command = Command as RoutedCommand;
                if (command != null)
                {
                    command.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    Command.Execute(CommandParameter);
                }                
            }
        }

        public static readonly DependencyProperty QueryValueProperty = DependencyProperty.Register(
            "QueryValue", typeof (string), typeof (AutoCompleteSearchTextBox), new FrameworkPropertyMetadata( // Property metadata
                string.Empty, // default value 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | // Flags 
                FrameworkPropertyMetadataOptions.Journal,
                null, // property changed callback 
                null,
                true, // IsAnimationProhibited
                UpdateSourceTrigger.LostFocus // DefaultUpdateSourceTrigger
                ));

        public string QueryValue
        {
            get { return (string) GetValue(QueryValueProperty); }
            set { SetValue(QueryValueProperty, value); }
        }

        public static readonly DependencyProperty IconBrushProperty = DependencyProperty.Register(
            "IconBrush", typeof (Brush), typeof (AutoCompleteSearchTextBox), new PropertyMetadata(default(Brush)));

        public Brush IconBrush
        {
            get { return (Brush) GetValue(IconBrushProperty); }
            set { SetValue(IconBrushProperty, value); }
        }

        public static readonly DependencyProperty SuggestBackgroundBrushProperty = DependencyProperty.Register(
            "SuggestBackgroundBrush", typeof (Brush), typeof (AutoCompleteSearchTextBox), new PropertyMetadata(default(Brush)));

        public Brush SuggestBackgroundBrush
        {
            get { return (Brush) GetValue(SuggestBackgroundBrushProperty); }
            set { SetValue(SuggestBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty SuggestHoverBrushProperty = DependencyProperty.Register(
            "SuggestHoverBrush", typeof (Brush), typeof (AutoCompleteSearchTextBox), new PropertyMetadata(default(Brush)));

        public Brush SuggestHoverBrush
        {
            get { return (Brush) GetValue(SuggestHoverBrushProperty); }
            set { SetValue(SuggestHoverBrushProperty, value); }
        }

        public static readonly DependencyProperty SuggestSelectedBrushProperty = DependencyProperty.Register(
            "SuggestSelectedBrush", typeof (Brush), typeof (AutoCompleteSearchTextBox), new PropertyMetadata(default(Brush)));

        public Brush SuggestSelectedBrush
        {
            get { return (Brush) GetValue(SuggestSelectedBrushProperty); }
            set { SetValue(SuggestSelectedBrushProperty, value); }
        }
    }
}
