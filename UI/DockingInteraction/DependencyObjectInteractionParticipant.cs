using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using XComponent.Common.UI.Properties;
using XComponent.Common.UI.Wpf;

namespace XComponent.Common.UI.DockingInteraction
{
    public abstract class DependencyObjectInteractionParticipant : DependencyObject, INotifyPropertyChanged, IInteractionParticipantWithContextMenu, IDisposable
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("PanelTitle", typeof(string), typeof(DependencyObjectInteractionParticipant),
            new PropertyMetadata(string.Empty, TitleChanged));

        private static readonly DependencyProperty SlaveTitleProperty = DependencyProperty.Register("SlaveTitle", typeof(string), typeof(DependencyObjectInteractionParticipant));

        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register("MenuItems", typeof(ObservableCollection<FrameworkElement>), typeof(DependencyObjectInteractionParticipant));

        protected abstract IEnumerable<Dictionary<string, object>> GetInteractionParameters(string targetViewType);

        public abstract bool HandleInteraction(string action, IEnumerable<Dictionary<string, object>> parameters);
        
        public abstract IEnumerable<IParticipantType> ContextMenuPeerTypes { get; }
        
        public abstract IEnumerable<IParticipantType> ContextMenuSlaveTypes { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private IInteractionMediator _mediator;
        private ParticipantIdentity _master;

        protected DependencyObjectInteractionParticipant(IParticipantType participantType, string name)
        {
            var identity = new ParticipantIdentity(participantType, name);
            Identity = identity;

            MenuItems = new ObservableCollection<FrameworkElement>();
        }

        protected DependencyObjectInteractionParticipant(IParticipantType participantType)
            : this(participantType, Guid.NewGuid().ToString())
        {
        }

        private static void TitleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = (IInteractionParticipant)dependencyObject;
            viewModel.Identity.UniqueName = (string)dependencyPropertyChangedEventArgs.NewValue;
        }

        public string PanelTitle
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public ParticipantIdentity Identity { get; private set; }

        public ObservableCollection<FrameworkElement> MenuItems
        {
            get { return (ObservableCollection<FrameworkElement>)GetValue(MenuItemsProperty); }
            set { SetValue(MenuItemsProperty, value); }
        }

        protected virtual void OnParticipantListChanged(object sender, EventArgs e)
        {
            if (Mediator == null)
            {
                return;
            }

            Func<object, ParticipantIdentity> identityAccessor = param => param as ParticipantIdentity;

            Func<object, IEnumerable<Dictionary<string, object>>> parameterAccessor = param =>
            {
                ParticipantIdentity selectedPeer = identityAccessor(param);
                if (selectedPeer == null)
                {
                    return Enumerable.Empty<Dictionary<string, object>>();
                }
                return GetInteractionParameters(selectedPeer.ParticipantType.Category);
            };

            var standardMenuItems = CommonContextMenuItemFactory.CreateStandardMenuItems(this, identityAccessor,
                parameterAccessor);

            Dispatcher.BeginInvoke((Action) (() => RebuildMenuItemList(standardMenuItems)));
        }

        protected void NotifySlaveParticipants(IEnumerable<Dictionary<string, object>> parameters, params IParticipantType[] targetTypes)
        {
            foreach (var targetType in targetTypes)
            {
                Mediator.RunMasterSlaveInteraction(Identity, targetType, CommonActions.UpdateDataSource, parameters);
            }
        }

        protected virtual void RebuildMenuItemList(IEnumerable<FrameworkElement> standardMenuItems)
        {
            var menuItems = new ObservableCollection<FrameworkElement>();
            foreach (FrameworkElement standardMenuItem in standardMenuItems)
            {
                if (standardMenuItem != null)
                {
                    menuItems.Add(standardMenuItem);
                }
            }

            IEnumerable<FrameworkElement> specificMenuItems = CreateSpecificMenuItems();
            foreach (FrameworkElement specificMenuItem in specificMenuItems)
            {
                menuItems.Add(specificMenuItem);
            }

            MenuItems = menuItems;
        }

        protected virtual IEnumerable<FrameworkElement> CreateSpecificMenuItems()
        {
            return Enumerable.Empty<FrameworkElement>();
        }

        public IInteractionMediator Mediator
        {
            get { return _mediator; }
            set
            {
                if (_mediator == value)
                {
                    return;
                }

                if (_mediator != null)
                {
                    _mediator.ParticipantsChanged -= OnParticipantListChanged;
                }

                _mediator = value;
                
                if (_mediator != null)
                {
                    _mediator.ParticipantsChanged += OnParticipantListChanged;
                }
            }
        }

        public string SlaveTitle
        {
            get { return (string)GetValue(SlaveTitleProperty); }
            set { SetValue(SlaveTitleProperty, value); }
        }

        public ParticipantIdentity Master
        {
            get { return _master; }
            set
            {
                if (_master != null)
                {
                    _master.Changed -= OnMasterIdentityChanged;
                }

                _master = value;
                
                if (_master != null)
                {
                    SetSlaveTitle();
                    _master.Changed += OnMasterIdentityChanged;
                }
                else
                {
                    SlaveTitle = string.Empty;
                }

                NotifyPropertyChanged(() => Master);
            }
        }

        private void SetSlaveTitle()
        {
            SlaveTitle = string.Format(": {0} {1}", Resources.EnslavedTo, Master.UniqueName);
        }

        private void OnMasterIdentityChanged(object sender, IdentityChangedEventArgs e)
        {
            SetSlaveTitle();
        }

        protected void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            this.NotifyPropertyChanged(PropertyChanged, property);                    
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mediator != null)
                {
                    _mediator.ParticipantsChanged -= OnParticipantListChanged;
                    _mediator = null;
                }

                Master = null;
            }
        }
    }
}
