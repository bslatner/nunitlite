using System.Diagnostics;
using NUnit.Framework.Api;

namespace NUnitLite.Api
{
    public class DebugOutputTestListener : ITestListener
    {
        private string _Pending = string.Empty;

        public void TestStarted(ITest test)
        {
            _Pending = string.Format("<{0}:", test.Name);
        }

        public void TestFinished(ITestResult result)
        {
            _Pending = _Pending + string.Format(":{0}>", result.ResultState);
            Debug.WriteLine(_Pending);
            _Pending = string.Empty;
        }

        public void TestOutput(TestOutput testOutput)
        {
            // do nothing
        }
    }
}
