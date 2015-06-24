// 
//    Copyright (c) INVIVOO Software. All rights reserved.
//    Licensed under the Apache license 2.0. See LICENSE file in the project root for full license information. 
// 

using System;
using System.Threading;
using NUnit.Framework;

namespace XComponent.UI.TestTools
{
    public static class AsyncCallbackUnitTest
    {
        public static void AssertWasCompleted(Action codeToTest, Func<bool> completionTestFunc, long timeout)
        {
            var mockContext = new DefaultSynchronizationContext();
            var oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(mockContext);
            try
            {
                mockContext.Post(obj => codeToTest(), null);
                if (!mockContext.RunMessagePump(completionTestFunc, timeout))
                {
                    Assert.Fail("Completion test failed");
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }
    }
}
