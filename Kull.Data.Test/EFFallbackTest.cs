using System;
using Kull.Data.EFFallback;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kull.Data.Test
{
    [TestClass]
    public class EFFallbackTest
    {
        [TestMethod]
        public void TestEFFallback()
        {
            var connStr = "metadata=res://*/Entities.csdl|res://*/SomeEntities.ssdl|res://*/Entities.msl;provider=System.Data.SqlClient;provider connection string=\"data source=anawesome.server.in.the.cloud;initial catalog=Testdatabase;persist security info=True;user id=auser;password=somerandomstring;asynchronous processing=True;multipleactiveresultsets=True;application name=EntityFramework\"";
            var res = ConnectionStringParser.ParseEF(connStr);
            
            Assert.AreEqual(res.Provider, "System.Data.SqlClient");
            Assert.AreEqual(res.ConnectionStringData["Application name"], "EntityFramework");
            var res2 = ConnectionStringParser.ParseEF(ConnectionStringParser.CreateEF(res.Provider, res.ConnectionString));
            
            Assert.AreEqual(res, res2);
            Assert.AreEqual(res.GetHashCode(), res2.GetHashCode());
            Assert.IsTrue(res == res2);
            Assert.IsFalse(res != res2);
            var res3 = ConnectionStringParser.ParseEF(res.ConnectionString);
            Assert.AreEqual(res3.ConnectionString, res.ConnectionString);
        }
    }
}
