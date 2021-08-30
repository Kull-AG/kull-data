using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;

namespace Kull.Data.Test
{
    [TestClass]
    public class RowHelperTests
    {
       public class TestClass
        {
            public string FirstName { get; set; }
            public string FamilyName { get; set; }
            public int? SomeId { get; set; }
        }

        public class TestClass2 : TestClass
        {
            public string MissingField { get; set; }
        }

        public record TestRecord(string FirstName, string FamilyName, int? SomeId);
        public record TestRecord2(string FirstName, string FamilyName, int? SomeId, string? MissingField);

        public class TestRecordWithConstr
        {
            public TestRecordWithConstr(DbDataReader reader, string firstName)
            {
                this.FirstName = firstName;
                var FamilyNameCol = reader.GetOrdinal("FamilyName");
                this.FamilyName = reader.IsDBNull(FamilyNameCol) ? null : reader.GetString(FamilyNameCol);
            }

            public string FirstName { get; }
            public string FamilyName { get; }
        }

        [TestMethod]
        public void TestSimpleCase()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestClass>(odr1, false).ToArray();
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.AreEqual(result[2].SomeId, 66);
        }

        [TestMethod]
        public void TestMissingField()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestClass2>(odr1, true).ToArray();
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
            var result = RowHelper.FromTable<TestRecord>(odr1).ToArray();
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.AreEqual(result[2].SomeId, 66);
        }

        [TestMethod]
        public void TestMissingFieldImmutable()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestRecord2>(odr1, true).ToArray();
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.IsNull(result[0].MissingField);
            Assert.AreEqual(result[2].SomeId, 66);
        }

        [TestMethod]
        public void TestReaderInjection()
        {
            var dt = DataReaderTests.GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            var result = RowHelper.FromTable<TestRecordWithConstr>(odr1, false).ToArray();
            Assert.AreEqual(result[0].FirstName, "peter");
            Assert.AreEqual(result[2].FamilyName, "oijo243");
        }

    }
}
