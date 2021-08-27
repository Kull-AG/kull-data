using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kull.Data.Test
{
    [TestClass]
    public class RowHelperTests
    {
       public class TestClass
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int? SomeId { get; set; }
        }

        public class TestClass2 : TestClass
        {
            public string MissingField { get; set; }
        }

        public record TestRecord(string FirstName, string LastName, int? SomeId);
        public record TestRecord2(string FirstName, string LastName, int? SomeId, string? MissingField);

        [TestMethod]
        public void TestSimpleCase()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestClass>(odr1, false);
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.AreEqual(result[2].SomeId, 66);
        }

        [TestMethod]
        public void TestMissingField()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestClass2>(odr1, true);
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.IsNull(result[0].MissingField);
            Assert.AreEqual(result[2].SomeId, 66);
        }

        [TestMethod]
        public void TestImmutable()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestRecord>(odr1);
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.AreEqual(result[2].SomeId, 66);
        }

        [TestMethod]
        public void TestMissingFieldImmutable()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestRecord2>(odr1, true);
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.IsNull(result[0].MissingField);
            Assert.AreEqual(result[2].SomeId, 66);
        }

    }
}
