using System;
using System.Collections.Generic;
using System.Windows;

namespace XComponent.Common.UI.Wpf
{
    public abstract class XCDelayedNotificationDependencyObject: DependencyObject, IDisposable
    {

        private readonly Dictionary<DependencyProperty, object> pendingNotifications = new Dictionary<DependencyProperty, object>();
		 public event Action DelayExpired;

        public XCDelayedNotificationDependencyObject()
        {
            XCNotifyScheduler.Instance.Register(this);
        }

        protected void SetValueDelayed(DependencyProperty property, object value)
        {
            lock(pendingNotifications)
            {
                if (!pendingNotifications.ContainsKey(property))
                {
                    pendingNotifications.Add(property, value);
                }
                else
                {
                    pendingNotifications[property] = value;
                }
            }
        }


        /// <summary>
        /// Enable PropertyChanged Events to fire again
        /// </summary>
        internal void Notify()
        {
         
            lock (pendingNotifications)
            {
               foreach(var keyPairVal in pendingNotifications)
               {
                   DependencyProperty property = keyPairVal.Key;
                   object value = keyPairVal.Value;
                   SetValue(property, value);
               }

                pendingNotifications.Clear();
            }
			if (DelayExpired != null)
            {
                DelayExpired();
            }
        }

        internal void BeginNotify()
        {
            Dispatcher.BeginInvoke((Action) Notify);
        }

        #region IDisposable Members

        virtual public void Dispose()
        {
            XCNotifyScheduler.Instance.UnRegister(this);
        }


        #endregion
    }
}
