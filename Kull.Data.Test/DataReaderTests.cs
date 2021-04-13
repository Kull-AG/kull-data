using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kull.Data.Test
{
    [TestClass]
    public class DataReaderTests
    {
        public static List<IReadOnlyDictionary<string, object>> GetTestDataSet()
        {
            return new List<IReadOnlyDictionary<string, object>>()
                {
                new Dictionary<string, object>()
                    {
                        {"FirstName", "peter" },
                        {"FamilyName", "Meier" },
                        { "Company", null },
                        { "SomeId", 123 }
                    },
                     new Dictionary<string, object>()
                    {
                        {"FirstName", "hansli" },
                        {"FamilyName", "Muster" },
                        { "Company", "Muster" },
                        { "SomeId", 4536 }
                    },
                     new Dictionary<string, object>()
                    {
                        {"FirstName", "bol" },
                        {"FamilyName", "oijo243" },
                        { "Company", null },
                        { "SomeId", 66 }
                    },
                     new Dictionary<string, object>()
                    {
                        {"FirstName", "peter" },
                        {"FamilyName", "Meier" },
                        { "Company", null },
                        { "SomeId", 377 }
                    }
                };
        }
        [TestMethod]
        public void TestObjectDataReader()
        {
            var dt = GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            while (odr1.Read())
            {
                Assert.AreEqual(fieldCount, odr1.FieldCount);
                
            }
        }

        [TestMethod]
        public void TestObjectDataReaderFields()
        {
            var dt = GetTestDataSet();
            int fieldCount = dt[0].Count;
            var odr1 = new Kull.Data.DataReader.ObjectDataReader(dt);
            odr1.Read();
            Assert.AreEqual(odr1.GetInt32(odr1.GetOrdinal("SomeId")), 123);
            Assert.AreEqual(odr1.GetString(odr1.GetOrdinal("FirstName")), "peter");
            var objects = new object[fieldCount];
            odr1.GetValues(objects);
            Assert.AreEqual(objects[odr1.GetOrdinal("SomeId")], 123);
        }



        [TestMethod]
        public void TestWrappedDataReader()
        {
            var odr = new Kull.Data.DataReader.ObjectDataReader(GetTestDataSet());

            var wrapped = new Kull.Data.DataReader.WrappedDataReader(odr, new Dictionary<string, object>()
            {
                {"Guid", Guid.NewGuid() }
            });

            while (wrapped.Read())
            {
                object[] vls = new object[wrapped.FieldCount];
                wrapped.GetValues(vls);
                foreach (var vl in vls)
                {
                    Console.Write(vl);
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
        }


        [TestMethod]
        public void TestObjectDataReaderTypes()
        {
            var odr = new Kull.Data.DataReader.ObjectDataReader(GetTestDataSet(), deepTypeScan: true);
            for (int i = 0; i < odr.FieldCount; i++)
            {
                Assert.IsNotNull(odr.GetFieldType(i));
            }
        }
    }
}
