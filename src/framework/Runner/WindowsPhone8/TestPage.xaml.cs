using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;
using NUnitLite.Api;

namespace NUnitLite.Runner.WindowsPhone8
{
    public partial class TestPage : UserControl
    {
        public Assembly callingAssembly;
        public ITestListener listener = TestListener.NULL;
        public ITestFilter testFilter = TestFilter.Empty;
        private ITestAssemblyRunner runner;
        private TextWriter writer;
        private ITestResult result;

        public TestPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.runner = new NUnitLiteTestAssemblyRunner(new NUnitLiteTestAssemblyBuilder());
            this.listener = new DebugOutputTestListener();
            this.callingAssembly = this.callingAssembly ?? Assembly.GetCallingAssembly();
            this.writer = new TextBlockWriter(this.ScratchArea);

            TextUI.WriteHeader(this.writer);
            TextUI.WriteRuntimeEnvironment(this.writer);

            if (!LoadTestAssembly())
                writer.WriteLine("No tests found in assembly {0}", GetAssemblyName(callingAssembly));
            else
            {
                Task.Factory.StartNew(ExecuteTests, TaskCreationOptions.LongRunning);
            }
        }

        private void Write(string message)
        {
            Dispatcher.BeginInvoke(() => writer.Write(message));
        }

        private void WriteLine(string message)
        {
            Dispatcher.BeginInvoke(() => writer.WriteLine(message));
        }

        #region Helper Methods

        private bool LoadTestAssembly()
        {
            return runner.Load(callingAssembly, new Dictionary<string, string>());
        }

        private string GetAssemblyName(Assembly assembly)
        {
            return new AssemblyName(assembly.FullName).Name;
        }

        private void ExecuteTests()
        {
            result = runner.Run(listener, testFilter);

            Dispatcher.BeginInvoke(ReportTestResults);
        }

        private void ReportTestResults()
        {
            var reporter = new ResultReporter(result, writer);
            reporter.ReportResults();
            var summary = reporter.Summary;
            writer.Flush();

            //this.Total.Text = summary.TestCount.ToString();
            //this.Failures.Text = summary.FailureCount.ToString();
            //this.Errors.Text = summary.ErrorCount.ToString();
            //this.NotRun.Text = summary.NotRunCount.ToString();
            //this.Passed.Text = summary.PassCount.ToString();
            //this.Inconclusive.Text = summary.InconclusiveCount.ToString();

            //this.Notice.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
