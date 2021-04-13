using System;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kull.Data.Test
{
    [TestClass]
    public class ConnectionTest
    {
        
        [TestMethod]
        public void TestConnection()
        {

            // In-memory database only exists while the connection is open
            Environment.SetEnvironmentVariable("CONNECTIONOFTEST", "DataSource=:memory:");
            var connection = Kull.Data.DatabaseUtils.GetConnectionFromConfig("CONNECTIONOFTEST", System.Data.SQLite.SQLiteFactory.Instance);
            Assert.IsInstanceOfType(connection, typeof(SQLiteConnection));
            {
                var cmd = connection.AssureOpen().CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }
            
        }

        [TestMethod]
        public void TestConnectionFromEnvironment()
        {

            // In-memory database only exists while the connection is open
            Environment.SetEnvironmentVariable("CONNECTIONOFTEST", "DataSource=:memory:");
            var connection = Kull.Data.DatabaseUtils.GetConnectionFromEnvironment("CONNECTIONOFTEST", true, System.Data.SQLite.SQLiteFactory.Instance);
            Assert.IsInstanceOfType(connection, typeof(SQLiteConnection));
            {
                var cmd = connection.AssureOpen().CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

        }

        public void TestConnectionFromEnvironmentWithProvider()
        {

            // In-memory database only exists while the connection is open
            Environment.SetEnvironmentVariable("CONNECTIONOFTEST", "Provider=\"System.Data.SQLite.SQLite\"DataSource=:memory:");
            var connection = Kull.Data.DatabaseUtils.GetConnectionFromEnvironment("CONNECTIONOFTEST", true, System.Data.SQLite.SQLiteFactory.Instance);
            Assert.IsInstanceOfType(connection, typeof(SQLiteConnection));
            {
                var cmd = connection.AssureOpen().CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

        }

    }
}
