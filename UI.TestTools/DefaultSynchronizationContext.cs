// 
//    Copyright (c) INVIVOO Software. All rights reserved.
//    Licensed under the Apache license 2.0. See LICENSE file in the project root for full license information. 
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace XComponent.UI.TestTools
{
    public class DefaultSynchronizationContext : SynchronizationContext
    {
        private readonly Queue<Action> messagesToProcess = new Queue<Action>();
        private readonly object syncHandle = new object();
        private bool isRunning = true;
        private const int WaitTimeout = 150;

        public override void Send(SendOrPostCallback codeToRun, object state)
        {
            this.Post(codeToRun, state);
        }

        public override void Post(SendOrPostCallback codeToRun, object state)
        {
            lock (syncHandle)
            {
                messagesToProcess.Enqueue(() => codeToRun(state));
                SignalContinue();
            }
        }

        public bool RunMessagePump(Func<bool> checkFunc, long timeout)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            while (CanContinue() && !checkFunc() && stopWatch.ElapsedMilliseconds <= timeout)
            {
                lock (syncHandle)
                {
                    while (CanContinue() && messagesToProcess.Count == 0 && !checkFunc() && stopWatch.ElapsedMilliseconds <= timeout)
                    {
                        Monitor.Wait(syncHandle, WaitTimeout);
                    }

                    if (messagesToProcess.Count > 0)
                    {
                        var nexToRun = messagesToProcess.Dequeue();
                        nexToRun();
                    }
                }
            }

            stopWatch.Stop();
            return checkFunc();
        }                

        private bool CanContinue()
        {
            lock (syncHandle)
            {
                return isRunning;
            }
        }

        public void Cancel()
        {
            lock (syncHandle)
            {
                isRunning = false;
                SignalContinue();
            }
        }

        private void SignalContinue()
        {
            Monitor.Pulse(syncHandle);
        }
    }
}
