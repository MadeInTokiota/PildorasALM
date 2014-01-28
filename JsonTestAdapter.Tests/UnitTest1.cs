using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonTestAdapter.Tests
{
    using System.Collections.Generic;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sut = JsonTestDiscoverer.GetTests(new List<string>() { "files/tests.json" }, null);
        }
    }
}
