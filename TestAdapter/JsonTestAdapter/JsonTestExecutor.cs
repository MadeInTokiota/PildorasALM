using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTestAdapter
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

    [ExtensionUri(JsonTestExecutor.ExecutorUriString)]
    public class JsonTestExecutor : ITestExecutor
    {
	
	
        public const string ExecutorUriString = "executor://JsonTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(JsonTestExecutor.ExecutorUriString);
        private bool mCancelled;

        public void RunTests(IEnumerable<string> sources, IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            IEnumerable<TestCase> tests = JsonTestDiscoverer.GetTests(sources, null);
            RunTests(tests, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext,
               IFrameworkHandle frameworkHandle)
        {
            mCancelled = false;

            foreach (TestCase test in tests)
            {
                if (mCancelled) break;

                var testResult = new TestResult(test);

                testResult.Outcome = (TestOutcome)test.GetPropertyValue(TestResultProperties.Outcome);
                frameworkHandle.RecordResult(testResult);
            }
        }

        public void Cancel()
        {
            mCancelled = true;
        }
    }
}
