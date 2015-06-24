using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Monads;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using FirstFloor.ModernUI.Presentation;
using Syncfusion.Linq;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using XComponent.Common.UI.Helpers;
using XComponent.Common.UI.I18n;

namespace XComponent.Common.UI.Docking
{
    public class XCDocking : DockingManager
    {
        public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register(
            "ApplicationName", typeof(string), typeof(XCDocking), new PropertyMetadata(default(string)));

        public string ApplicationName
        {
            get { return (string)GetValue(ApplicationNameProperty); }
            set { SetValue(ApplicationNameProperty, value); }
        }

        public static readonly int DefaultLayoutVersion = 0;
        public static readonly int CurrentLayoutVersion = 3;

        static public readonly string ViewModelSaveFileName = "docking_models.xml";
        static public readonly string LayoutFileName = "docking_layout.xml";
        private bool isLayoutLoaded = true;
        private FrameworkElement _previousSelectedElement = null;

        static XCDocking()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XCDocking), new FrameworkPropertyMetadata(typeof(XCDocking)));

            ViewModelSaveFileName = string.Format("docking_models_{0}.xml", CultureManager.CurrentCulture);
            LayoutFileName = string.Format("docking_layout_{0}.xml", CultureManager.CurrentCulture);
        }

        public XCDocking()
        {
            this.DataTemplateSelector = new DefaultDataTemplateSelector();


            this.ActiveWindowChanged += (sender, args) =>
            {

                var element = args.NewValue as FrameworkElement;

                if (_currentActivatingControl != element)
                {
                    if (element != null && element.DataContext != this.ItemSelected)
                    {
                        _previousSelectedElement = _currentActivatingControl;
                        this.ItemSelected = this.ItemsSource.OfType<Object>().FirstOrDefault(e => e == element.DataContext);
                    }
                }
                _currentActivatingControl = null;

            };

            this.TabClosed += (sender, args) => CloseTabAction(args.TargetTabItem);

            this.CloseAllTabs += (sender, args) => args.ClosingTabItems.OfType<TabItemExt>().ToList().ForEach(CloseTabAction);

            this.CloseOtherTabs += (sender, args) => args.ClosingTabItems.OfType<TabItemExt>().ToList().ForEach(CloseTabAction);

            this.WindowClosing += (sender, args) =>
            {
                var itemToRemove = this.ItemsSource.OfType<Object>().FirstOrDefault(e => e == args.TargetItem.DataContext);
                this.ItemsSource.Remove(itemToRemove);
            };

            this.Unloaded += (sender, args) => this.ItemsSource.Do(collection =>
            {
                var notifyCollection = collection as INotifyCollectionChanged;
                if (notifyCollection != null)
                {
                    notifyCollection.CollectionChanged -= OnItemCollectionChanged;
                }
            });

            this.Loaded += (sender, args) => this.OnItemsSourcePropertyChanged(null, this.ItemsSource);
        }

        private void CloseTabAction(TabItemExt tab)
        {
            var elContext = ((ContentPresenter)tab.Content).DataContext;
            if (elContext == null && _previousSelectedElement != null) //temporary awful fix!!!
            {
                elContext = _previousSelectedElement.DataContext;
            }
            if (elContext != null)
            {
                var graphicalEl = this.Children.OfType<FrameworkElement>().First(e => e.DataContext == elContext);
                graphicalEl.Loaded -= NewFrameworkElementOnLoaded;
                this.ItemsSource.Remove(elContext);
            }
           
        }

        private void DisposeUiResource(FrameworkElement element)
        {
            var disposeEl = element as IDisposable;
            disposeEl.Do(e => e.Dispose());
            var disposeDataContext = element.DataContext as IDisposable;
            disposeDataContext.Do(e => e.Dispose());
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IList), typeof(XCDocking), new PropertyMetadata(default(IList), ItemsSourcePropertyChangedCallback));

        private static void ItemsSourcePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as XCDocking;
            control.Do(t => t.OnItemsSourcePropertyChanged((IList)dependencyPropertyChangedEventArgs.OldValue, (IList)dependencyPropertyChangedEventArgs.NewValue));
        }

        private void OnItemsSourcePropertyChanged(IList oldValue, IList newValue)
        {
            if (this.IsLoaded)
            {
                var oldCollection = oldValue as INotifyCollectionChanged;
                var newCollection = newValue as INotifyCollectionChanged;
                if (oldCollection != null)
                {
                    oldCollection.CollectionChanged -= OnItemCollectionChanged;
                }
                this.ResetUiDocuments();

                if (newCollection != null)
                {
                    newCollection.CollectionChanged += OnItemCollectionChanged;
                }

                this.AddNewUiDocuments(newValue);
            }
        }

        private void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (this.isLayoutLoaded)
            {
                switch (notifyCollectionChangedEventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.AddNewUiDocuments(notifyCollectionChangedEventArgs.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveUiDocuments(notifyCollectionChangedEventArgs.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.RemoveUiDocuments(notifyCollectionChangedEventArgs.OldItems);
                        this.AddNewUiDocuments(notifyCollectionChangedEventArgs.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.ResetUiDocuments();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void AddNewUiDocuments(IEnumerable viewModelsToAdd)
        {
            if (viewModelsToAdd == null) return;

            ((DocumentContainer)this.DocContainer).AddTabDocumentAtLast = true;

            foreach (var newItem in viewModelsToAdd)
            {
                var selectedDataTemplate = this.DataTemplateSelector.SelectTemplate(newItem, this);
                selectedDataTemplate.Do(d =>
                {
                    var newFrameworkElement = d.LoadContent() as FrameworkElement;
                    newFrameworkElement.Do(element =>
                    {
                        element.DataContext = newItem;
                        if (String.IsNullOrWhiteSpace(element.Name))
                        {
                            element.Name = "ctrlname" + Guid.NewGuid().ToString("N").Substring(0, 8);
                        }
                    });

                    newFrameworkElement.Loaded += NewFrameworkElementOnLoaded;
                    UpdatePanelContextMenu(newFrameworkElement);
                    this.Children.Add(newFrameworkElement);
                    this.ItemSelected = newFrameworkElement.DataContext;
                });
            }
        }

        private void UpdatePanelContextMenu(FrameworkElement control)
        {
            var dockingPanelTitle = control.DataContext as IDockingPanelTitle;

            if (dockingPanelTitle != null)
            {
                var collection = new DocumentTabItemMenuItemCollection();
                var customCollection = new CustomMenuItemCollection();

                var customMenuItem = new CustomMenuItem();

                customMenuItem.Header = Properties.Resources.ResourceManager.GetString("RenameTooltip");
                var command = new RelayCommand(p =>
                {
                    var renamingWindow = new RenamingWindow(dockingPanelTitle.PanelTitle);
                    renamingWindow.Owner = Application.Current.MainWindow;
                    var res = renamingWindow.ShowDialog();
                    if (res != null && res.Value)
                    {
                        dockingPanelTitle.PanelTitle = renamingWindow.PanelTitle;
                    }
                });

                customMenuItem.Command = command;

                collection.Add(customMenuItem);
                customCollection.Add(customMenuItem);
                SetDocumentTabItemContextMenuItems(control, collection);
                SetDockWindowContextMenuItems(control, customCollection);
                SetFloatWindowContextMenuItems(control, customCollection);
            }
        }

        private void NewFrameworkElementOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var el = sender as FrameworkElement;
            var border = WpfHelper.FindAncestor(el, typeof(Border3D)) as Border3D;
            if (border != null)
            {
                if (TabDocumentBorderBrush != null)
                {
                    border.BorderBrush = TabDocumentBorderBrush;
                    border.BorderThickness = new Thickness(1, 0, 1, 1);
                }
            }
            //Hack to set data context for header
            var dockPanel = WpfHelper.FindAncestor(el, typeof(DockPanel)) as DockPanel;
            if (dockPanel != null)
            {
                dockPanel.DataContext = el;
                //Hack for specific case of floating control
                var dockElement = WpfHelper.FindAncestor(dockPanel, typeof(DockPanel)) as DockPanel;
                if (dockElement != null)
                {
                    dockElement.DataContext = el;
                }
            }

        }

        private void RemoveUiDocuments(IEnumerable viewModelsToRemove)
        {
            if (viewModelsToRemove == null) return;
            foreach (var oldItem in viewModelsToRemove)
            {
                this.Children.OfType<FrameworkElement>().Where(e => e.DataContext == oldItem).ToList()
                    .ForEach(t =>
                    {
                        this.DisposeUiResource(t);
                        this.Children.Remove(t);
                        if (this.Children.Count == 0)
                        {
                            this.ItemSelected = null;
                        }
                    });
            }
        }

        private void ResetUiDocuments()
        {
            var elements = this.Children.OfType<FrameworkElement>().ToList();
            this.Children.Clear();
            elements.ForEach(this.DisposeUiResource);
            this.ItemSelected = null;
        }

        /// <summary>
        /// List of view models
        /// </summary>
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set
            {
                if (value == null)
                {
                    ClearValue(ItemsSourceProperty);
                    this.ResetUiDocuments();
                }
                else
                {
                    SetValue(ItemsSourceProperty, value);
                }
            }
        }

        public static readonly DependencyProperty DataTemplateSelectorProperty = DependencyProperty.Register(
            "DataTemplateSelector", typeof(DataTemplateSelector), typeof(XCDocking), new PropertyMetadata(default(DataTemplateSelector)));

        public DataTemplateSelector DataTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DataTemplateSelectorProperty); }
            set { SetValue(DataTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemSelectedProperty = DependencyProperty.Register(
            "ItemSelected", typeof(Object), typeof(XCDocking), new PropertyMetadata(default(Object), ItemSelectedPropertyChangedCallback));

        private FrameworkElement _currentActivatingControl;
        private static void ItemSelectedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as XCDocking;
            control.Do(e =>
            {
                var elementToSelect = e.Children.OfType<FrameworkElement>().FirstOrDefault(c => c.DataContext == dependencyPropertyChangedEventArgs.NewValue);
                if (elementToSelect != null)
                {
                    e.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)(() =>
                    {
                        e._currentActivatingControl = elementToSelect;
                        e.ActivateWindow(elementToSelect.Name);
                        DockedElementTabbedHost host = GetHost(elementToSelect, GetState(elementToSelect));
                        if (host != null)
                            host.Focus();
                    }));
                }
            });
        }

        public Object ItemSelected
        {
            get { return GetValue(ItemSelectedProperty); }
            set { SetValue(ItemSelectedProperty, value); }
        }

        private string GetViewModelCustomParameters(object viewModel)
        {
            using (var writer = new StringWriter())
            {
                this.PanelViewModelBuilders[GetDockingKey(viewModel)].SaveViewModel(viewModel, writer);
                return writer.ToString();
            }
        }

        public static readonly DependencyProperty PanelViewModelBuildersProperty = DependencyProperty.Register(
            "PanelViewModelBuilders", typeof(Dictionary<string, IDockingPanelViewModelBuilder>), typeof(XCDocking), new PropertyMetadata(default(Dictionary<string, IDockingPanelViewModelBuilder>)));

        public Dictionary<string, IDockingPanelViewModelBuilder> PanelViewModelBuilders
        {
            get { return (Dictionary<string, IDockingPanelViewModelBuilder>)GetValue(PanelViewModelBuildersProperty); }
            set { SetValue(PanelViewModelBuildersProperty, value); }
        }

        private static string GetDockingKey(object viewModel)
        {
            return viewModel is IDockingKey ? ((IDockingKey)viewModel).DockingKey : viewModel.GetType().FullName;
        }

        public void SaveLayout(string folderPath)
        {
            var xmlSerializer = new XmlSerializer(typeof(XCDockingLayout));
            var parameters = this.ItemsSource.OfType<object>().Where(o => this.PanelViewModelBuilders.ContainsKey(GetDockingKey(o))).Select(e =>
            {
                var customParameter = GetViewModelCustomParameters(e);
                var dockingPanelTitle = e as IDockingPanelTitle;
                return new ViewModelSavedParameters
                    (CurrentLayoutVersion, this.Children.OfType<FrameworkElement>().First(c => c.DataContext == e).Name,
                        GetDockingKey(e),
                        dockingPanelTitle != null && !string.IsNullOrWhiteSpace(dockingPanelTitle.PanelTitle) ? dockingPanelTitle.PanelTitle : null,
                        customParameter
                    );
            }).ToList();

            var dockingLayout = new XCDockingLayout
            {
                LayoutVersion = CurrentLayoutVersion,
                ViewModelSavedParameters = parameters,
            };

            using (var wr = new StreamWriter(Path.Combine(folderPath, ViewModelSaveFileName), false))
            {
                xmlSerializer.Serialize(wr, dockingLayout);
            }

            var binaryFormatter = new BinaryFormatter();
            this.SaveDockState(binaryFormatter, StorageFormat.Xml, Path.Combine(folderPath, LayoutFileName));
        }

        public void LoadLayout(string folderPath, string oldFolderPath = "")
        {
            var modelPath = Path.Combine(folderPath, ViewModelSaveFileName);
            var layoutPath = Path.Combine(folderPath, LayoutFileName);

            if (!string.IsNullOrEmpty(oldFolderPath))
            {
                if (!File.Exists(modelPath))
                {
                    var oldModelPath = Path.Combine(oldFolderPath, ViewModelSaveFileName);

                    if (File.Exists(oldModelPath))
                    {
                        File.Copy(oldModelPath, modelPath);
                    }
                }

                if (!File.Exists(layoutPath))
                {
                    var oldLayoutPath = Path.Combine(oldFolderPath, LayoutFileName);

                    if (File.Exists(oldLayoutPath))
                    {
                        File.Copy(oldLayoutPath, layoutPath);
                    }
                }
            }

            if (!File.Exists(modelPath) || !File.Exists(layoutPath) || this.PanelViewModelBuilders == null)
                return;

            XCDockingLayout dockingLayoutModel;
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(XCDockingLayout));
                using (var reader = new StreamReader(modelPath))
                {
                    dockingLayoutModel = xmlSerializer.Deserialize(reader) as XCDockingLayout;
                }
            }
            catch
            {
                dockingLayoutModel = new XCDockingLayout
                {
                    LayoutVersion = DefaultLayoutVersion,
                };

                // Handle format in use before the layout versioning system
                var xmlSerializer = new XmlSerializer(typeof(List<ViewModelSavedParameters>));
                using (var reader = new StreamReader(modelPath))
                {
                    dockingLayoutModel.ViewModelSavedParameters = xmlSerializer.Deserialize(reader) as List<ViewModelSavedParameters>;
                }
            }

            List<ViewModelSavedParameters> viewModelsParameters = dockingLayoutModel.ViewModelSavedParameters;
            if (viewModelsParameters == null
                || viewModelsParameters.Count == 0
                || (viewModelsParameters.Count(e => this.PanelViewModelBuilders.ContainsKey(e.Type)) == 0))
                return;

            this.ItemsSource.Clear();

            this.isLayoutLoaded = false;

            string dockingLayout = File.ReadAllText(Path.GetFullPath(layoutPath));
            string migratedDockingLayout = MigrateDockingLayout(dockingLayout, dockingLayoutModel.LayoutVersion);
            using (var xmlTextReader = new XmlTextReader(new StringReader(migratedDockingLayout)))
            {
                var defaultXmlSerializer = CreateDefaultXmlSerializer(typeof(List<DockingParams>));

                if (defaultXmlSerializer.CanDeserialize(xmlTextReader))
                {
                    try
                    {
                        var parameters = ((List<DockingParams>)defaultXmlSerializer.Deserialize(xmlTextReader)).Where(e => viewModelsParameters.Any(t => t.PanelName == e.Name)).ToList();

                        foreach (var dockingParams in parameters)
                        {
                            var viewModelParam = viewModelsParameters.First(e => e.PanelName == dockingParams.Name);
                            if (this.PanelViewModelBuilders.ContainsKey(viewModelParam.Type))
                            {
                                object viewModel;

                                IDockingPanelViewModelBuilder viewModelBuilder = this.PanelViewModelBuilders[viewModelParam.Type];
                                int layoutVersion = viewModelParam.LayoutVersion;
                                string migratedParameters = viewModelBuilder.MigrateParameters(viewModelParam.CustomParametersStr, layoutVersion);
                                using (var reader = new StringReader(migratedParameters))
                                {
                                    viewModel = viewModelBuilder.CreateViewModel(reader);
                                    var dockingPanelTitle = viewModel as IDockingPanelTitle;
                                    if (dockingPanelTitle != null && !string.IsNullOrWhiteSpace(viewModelParam.PanelTitle))
                                    {
                                        dockingPanelTitle.PanelTitle = viewModelParam.PanelTitle;
                                    }
                                }

                                if (viewModel != null)
                                {
                                    var selectedDataTemplate = this.DataTemplateSelector.SelectTemplate(viewModel, this);
                                    selectedDataTemplate.Do(d =>
                                    {
                                        var newFrameworkElement = d.LoadContent() as FrameworkElement;
                                        newFrameworkElement.Do(element =>
                                        {
                                            element.DataContext = viewModel;
                                            element.Name = dockingParams.Name;
                                        });

                                        newFrameworkElement.Loaded += NewFrameworkElementOnLoaded;
                                        UpdatePanelContextMenu(newFrameworkElement);
                                        this.Children.Add(newFrameworkElement);
                                    });

                                    this.ItemsSource.Add(viewModel);
                                }
                            }
                        }

                        var stream = new MemoryStream();
                        using (var reader = new XmlTextWriter(stream, Encoding.Default))
                        {
                            defaultXmlSerializer.Serialize(reader, parameters);
                            reader.Flush();

                            using (var loadReader = new XmlTextReader(new MemoryStream(stream.GetBuffer())))
                            {
                                this.LoadDockState(loadReader);
                            }
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (ex.InnerException is XmlException)
                            throw ex.InnerException;
                        throw;
                    }
                    finally
                    {
                        this.isLayoutLoaded = true;
                    }
                }
            }
        }

        private string MigrateDockingLayout(string dockingLayout, int layoutVersion)
        {
            return dockingLayout;
        }

        public static readonly DependencyProperty TabDocumentBorderBrushProperty = DependencyProperty.Register(
            "TabDocumentBorderBrush", typeof(Brush), typeof(XCDocking), new PropertyMetadata(null));

        public Brush TabDocumentBorderBrush
        {
            get { return (Brush)GetValue(TabDocumentBorderBrushProperty); }
            set { SetValue(TabDocumentBorderBrushProperty, value); }
        }
    }
}
