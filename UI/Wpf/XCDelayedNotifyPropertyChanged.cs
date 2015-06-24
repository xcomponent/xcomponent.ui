using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace XComponent.Common.UI.Wpf
{
    public abstract class XCDelayedNotifyPropertyChanged : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly HashSet<string> hashSet = new HashSet<string>();
        private bool isRegistered = false;

        protected XCDelayedNotifyPropertyChanged()
        {
            RegisterNotifyPropertyChanged();
        }

        protected XCDelayedNotifyPropertyChanged(bool bRegister)
        {
            if (bRegister)
            {
                RegisterNotifyPropertyChanged();
            }
        }

        public void RegisterNotifyPropertyChanged()
        {
            if (!isRegistered)
            {
                XCNotifyScheduler.Instance.Register(this);
                isRegistered = true;
            }
        }

        protected void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            RaisePropertyChanged(WPFExtensions.PropertyName(property));
        }

       
        protected void RaisePropertyChanged(string propertyName)
        {
            lock (hashSet)
            {
                if (!hashSet.Contains(propertyName))
                {
                    hashSet.Add(propertyName);
                }
            }
        }

        protected void NotifyNoDelay<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyNoDelay(WPFExtensions.PropertyName(property));
        }

        protected void NotifyNoDelay(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Enable PropertyChanged Events to fire again
        /// </summary>
        internal void Notify()
        {
            List<string> events = new List<string>();
            lock (hashSet)
            {
                events.AddRange(hashSet.ToArray());
                hashSet.Clear();
            }

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                foreach (string propertyName in events)
                {
                    if (handler != null && !string.IsNullOrEmpty(propertyName))
                    {
                        handler(this, new PropertyChangedEventArgs(propertyName));
                    }
                }
            }

        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get;
            set;
        }

        #region IDisposable Members

        [Browsable(false)]
        virtual public void Dispose()
        {
            IsDisposed = true;
            if (isRegistered)
            {
                XCNotifyScheduler.Instance.UnRegister(this);                
            }
        }


        #endregion
    }
}
