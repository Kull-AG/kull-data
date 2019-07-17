using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kull.Data.Test
{
    /// <summary>
    /// Summary description for StringUtilsTest
    /// </summary>
    [TestClass]
    public class StringUtilsTest
    {
     
        [TestMethod]
        public void TestReplaceInsensitive()
        {
            var output = "tester Lorem ipsum Dolor lorem Lodem".ReplaceInsensitive("lOREM", "bla{{Found}}");
            Assert.AreEqual(output, "tester blaLorem ipsum Dolor blalorem Lodem");
            Assert.AreEqual("".ReplaceInsensitive(null, null), "");
        }
    }
}
