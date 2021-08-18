using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kull.Data.Test
{
    [TestClass]
    public class RepotCallTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var rc = new Reporting.ReportCall("testreport");
            rc.SetParameter("Test1", true)
                .SetParameter("Test2", "hello")
                .SetParameter("Test2", "_no_random_value")
                .SetParameter("Test3", 234);
            rc.ReportFormat = Reporting.ReportFormat.Excel;
            var url = rc.GetUrl().ToString();
            Assert.IsTrue(url.Contains("_no_random_value"));
            Assert.IsTrue(url.Contains("EXCELOPENXML"));
            Assert.IsFalse(url.Contains("hello"));
            var copy = new Reporting.ReportFormat(Reporting.ReportFormat.PDF.DisplayName,
                Reporting.ReportFormat.PDF.RSParam,
                Reporting.ReportFormat.PDF.Extension);
            Assert.AreEqual(copy, Reporting.ReportFormat.PDF);
        }
    }
}
