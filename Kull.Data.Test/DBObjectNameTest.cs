using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kull.Data.Test
{
    [TestClass]
    public class DBObjectNameTest
    {
        [TestMethod]
        public void TestKeyword()
        {
            DBObjectName dBObjectName = new DBObjectName("dbo", "SelEct");
            Assert.AreEqual(dBObjectName.ToString(false, true), "dbo.\"SelEct\"");
        }

        [TestMethod]
        public void TestKeyword2()
        {
            DBObjectName dBObjectName = "dbo.SelEct";
            Assert.AreEqual(dBObjectName.ToString(false, true), "dbo.\"SelEct\"");
        }

        [TestMethod]
        public void TestNormal()
        {
            DBObjectName dBObjectName = new DBObjectName("dbo", "SelEct_Things");
            Assert.AreEqual(dBObjectName.ToString(false, true), "dbo.SelEct_Things");
        }

        [TestMethod]
        public void TestSpace()
        {
            DBObjectName dBObjectName = new DBObjectName("dbo", "taes saf");
            Assert.AreEqual(dBObjectName.ToString(false, true), "dbo.\"taes saf\"");
        }
    }
}
