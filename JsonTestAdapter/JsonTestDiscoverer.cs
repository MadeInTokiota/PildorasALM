using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTestAdapter
{
    using System.IO;

    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    using Newtonsoft.Json;

    [DefaultExecutorUri(JsonTestExecutor.ExecutorUriString)]
    [FileExtension(".json")]
    public class JsonTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources,
                                    IDiscoveryContext discoveryContext,
                                    IMessageLogger logger,
                                    ITestCaseDiscoverySink discoverySink)
        {
            GetTests(sources, discoverySink);
        }

        public static List<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var testList = new List<TestCase>();
            foreach (var source in sources)
            {
                using (var sr = new StreamReader(source))
                {
                    var json = sr.ReadToEnd();
                    var tests = JsonConvert.DeserializeObject<List<Test>>(json);

                    foreach (var test in tests)
                    {
                        if (string.IsNullOrEmpty(test.Name))
                        {
                            continue;
                        }

                        var testCase = new TestCase(test.Name, JsonTestExecutor.ExecutorUri, source)
                                           {
                                               CodeFilePath =
                                                   source,
                                           };

                        if (discoverySink != null)
                        {
                            discoverySink.SendTestCase(testCase);
                        }
                        else
                        {
                            TestOutcome outcome;
                            Enum.TryParse<TestOutcome>(test.Outcome, out outcome);
                            testCase.SetPropertyValue(TestResultProperties.Outcome, outcome);
                        }

                        testList.Add(testCase);
                    }
                }
            }
            return testList;
        }
    }
}
