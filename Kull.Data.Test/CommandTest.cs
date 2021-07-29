using System;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kull.Data.Test
{
    [TestClass]
    public class CommandTest
    {
        class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        [TestMethod]
        public void TestCommands()
        {

            // In-memory database only exists while the connection is open
            var connection = new SQLiteConnection("DataSource=:memory:");

            {
                var cmd = connection.AssureOpen().CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "CREATE TABLE tester (id INTEGER, name varchar(100))";
                cmd.ExecuteNonQuery();
            }
            {
                var cmd = connection.AssureOpen().CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO tester (id, name) VALUES(@id, @name)";
                cmd.AddCommandParameter("id", 1)
                    .AddCommandParameter("@name", "hello");
                cmd.ExecuteNonQuery();
            }
            {
                var cmd = connection.AssureOpen().CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT id as Id, name as Name FROM tester where id=@id and name <> @name";
                cmd.AddCommandParameter("id", 1);
                cmd.AddParametersFromEntity(new { name = "not hello" });

                var dt = cmd.AsArrayOf<TestData>();
                Assert.AreEqual(dt.Length, 1);
                Assert.AreEqual(dt[0].Name, "hello");
                Assert.AreEqual(dt[0].Id, 1);
            }
            {
                var cmd = connection.AssureOpen().CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT id as Id, name as Name FROM tester";
                var ds = cmd.ReadAsDataSet();
                Assert.AreEqual(ds.Tables.Count, 1);
                Assert.AreEqual(ds.Tables[0].Rows[0]["name"], "hello");
            }
        }

        [TestMethod]
        public void TestDBObjectName()
        {
            var tester1 = DBObjectName.FromString("Test.[dbo].hello");
            var tester2 = DBObjectName.FromString("Test.dbo.\"hello\"");
            Assert.AreEqual(tester1, tester2);
            Assert.AreEqual("Test", tester1.DataBaseName);
            Assert.AreEqual("dbo", tester1.Schema);
            Assert.AreEqual("hello", tester1.Name);
            var tester3 = DBObjectName.FromString("[hallo bla]");
            var tester4 = DBObjectName.FromString("\"hallo bla\"");
            Assert.AreEqual(tester3, tester4);


            var tester5 = DBObjectName.FromString("test.[hallo .- bla]");
            Assert.AreEqual(tester5.Name, "hallo .- bla");
            Assert.AreEqual(tester5.Schema, "test");


            var tester6 = DBObjectName.FromString("test.\"asdf[hallo .- bla]f\"");
            Assert.AreEqual(tester6.Name, "asdf[hallo .- bla]f");
            Assert.AreEqual(tester6.Schema, "test");
        }
    }
}
