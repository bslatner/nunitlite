using System;
using System.Windows.Controls;
using System.Windows.Documents;
using NUnit.Framework.Api;

namespace NUnitLite.Runner.WindowsPhone8
{
    public class TextBlockTestListener : ITestListener
    {
        private string _Pending = string.Empty;
        private readonly TextBlock _TextBlock;

        public TextBlockTestListener(TextBlock textBlock)
        {
            if (textBlock == null) throw new ArgumentNullException("textBlock");

            _TextBlock = textBlock;
        }

        public void TestStarted(ITest test)
        {
            _Pending = string.Format("<{0}:", test.Name);
        }

        public void TestFinished(ITestResult result)
        {
            _Pending = _Pending + string.Format(":{0}>", result.ResultState);
            _TextBlock.Inlines.Add(_Pending);
            _TextBlock.Inlines.Add(new LineBreak());
        }

        public void TestOutput(TestOutput testOutput)
        {
            // do nothing
        }
    }
}
