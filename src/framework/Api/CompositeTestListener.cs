using System;
using System.Collections.Generic;

namespace NUnit.Framework.Api
{
    public class CompositeTestListener : ITestListener
    {
        private readonly List<ITestListener> _OtherListeners = new List<ITestListener>();

        public CompositeTestListener()
        {
        }

        public CompositeTestListener(IEnumerable<ITestListener> listeners)
        {
            if (listeners == null) throw new ArgumentNullException("listeners");
            _OtherListeners.AddRange(listeners);
        }

        public void Add(ITestListener listener)
        {
            if (listener == null) throw new ArgumentNullException("listener");
            _OtherListeners.Add(listener);
        }

        public void TestStarted(ITest test)
        {
            foreach (var l in _OtherListeners)
            {
                l.TestStarted(test);
            }
        }

        public void TestFinished(ITestResult result)
        {
            foreach (var l in _OtherListeners)
            {
                l.TestFinished(result);
            }
        }

        public void TestOutput(TestOutput testOutput)
        {
            foreach (var l in _OtherListeners)
            {
                l.TestOutput(testOutput);
            }
        }
    }
}