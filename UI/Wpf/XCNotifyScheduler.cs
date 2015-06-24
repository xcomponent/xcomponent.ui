using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace XComponent.Common.UI.Wpf
{
    public class XCNotifyScheduler:IDisposable
    {
        public const int DefaultRefreshRateInMmms = 250;
        private static readonly object InstanceLocker = new Object();
        private static volatile XCNotifyScheduler instance;

        readonly private List<XCDelayedNotifyPropertyChanged> delayedNotifyList = new List<XCDelayedNotifyPropertyChanged>();
        readonly private List<XCDelayedNotificationDependencyObject> delayedDependencyPropertyList = new List<XCDelayedNotificationDependencyObject>();
        readonly private DispatcherTimer notifyTimer;

        public event Action OnSchedulerElapsed;

        private int refreshRate = DefaultRefreshRateInMmms;

        public int RefreshRate
        {
            get
            {
                return refreshRate;
            }
            set
            {
                if (refreshRate != value)
                {
                    refreshRate = value;

                    lock (notifyTimer)
                    {
                        
                        notifyTimer.Stop();
                        notifyTimer.Interval = TimeSpan.FromMilliseconds(refreshRate);
                        notifyTimer.Start();
                    }
                }
            }
        }

        private XCNotifyScheduler()
        {
            notifyTimer = new DispatcherTimer(DispatcherPriority.Background);
            notifyTimer.Interval = TimeSpan.FromMilliseconds(refreshRate);
            notifyTimer.Tick += new EventHandler(OnTimer);
            notifyTimer.Start();
        }


        public static XCNotifyScheduler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (InstanceLocker)
                    {
                        if (instance == null)
                            instance = new XCNotifyScheduler();
                    }
                }

                return instance;

            }
        }

        public void Register(XCDelayedNotificationDependencyObject notify)
        {
            lock (delayedDependencyPropertyList)
            {
                delayedDependencyPropertyList.Add(notify);
            }
        }

        public void Register(XCDelayedNotifyPropertyChanged notify)
        {
            lock (delayedNotifyList)
            {
                delayedNotifyList.Add(notify);
            }
        }

        public void UnRegister(XCDelayedNotificationDependencyObject notify)
        {
            lock (delayedDependencyPropertyList)
            {
                if (delayedDependencyPropertyList.Contains(notify))
                {
                    delayedDependencyPropertyList.Remove(notify);
                }
            }
        }

        public void UnRegister(XCDelayedNotifyPropertyChanged notify)
        {
            lock (delayedNotifyList)
            {
                if (delayedNotifyList.Contains(notify))
                {
                    delayedNotifyList.Remove(notify);
                }
            }
        }


        private void OnTimer(Object sender, EventArgs args)
        {
            NotifyAll();
        }

        private void NotifyAll()
        {
            lock (delayedNotifyList)
            {
                foreach (var delayedNotifyPropertyChanged in delayedNotifyList)
                {
                    delayedNotifyPropertyChanged.Notify();
                }
            }

            lock (delayedDependencyPropertyList)
            {
                foreach (var delayedDependencyProperty in delayedDependencyPropertyList)
                {
                    delayedDependencyProperty.Notify();
                }
            }

            if (OnSchedulerElapsed != null)
            {
                OnSchedulerElapsed();
            }
        }


        public void Dispose()
        {
            notifyTimer.Stop();
            delayedDependencyPropertyList.Clear();
            delayedNotifyList.Clear();
            instance = null;
        }
    }
}
