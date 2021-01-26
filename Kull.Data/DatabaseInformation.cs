using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kull.Data
{
    /// <summary>
    /// A Utility Class for Database Scripting
    /// </summary>
    public class DatabaseInformation
    {
        /// <summary>
        /// A Dictionary with the name of the stored procedure as key and the names of its parameters as its value
        /// </summary>
        private readonly ConcurrentDictionary<string, string[]> spParameters = new ConcurrentDictionary<string, string[]>();

        private static readonly  ConcurrentDictionary<DbConnection, DatabaseInformation> connectionInfoCache = new ConcurrentDictionary<DbConnection, DatabaseInformation>();

        /// <summary>
        /// Clear all Cached Parameters of Stored Procedures
        /// </summary>
        public void ClearCachedSPParameters()
        {
            spParameters.Clear();
        }



        /// <summary>
        /// The database Connection
        /// </summary>
        public DbConnection Connection { get;  }

        
        /// <summary>
        /// Creates a new DatabaseInformation Instance with the specified connection
        /// </summary>
        /// <param name="connection"></param>
        private DatabaseInformation(DbConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Gets an DatabaseInformation Object from the cache or creates a new one
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static DatabaseInformation Create(DbConnection connection)
        {
            return connectionInfoCache.GetOrAdd(connection, c => new DatabaseInformation(c));
        }

        /// <summary>
        /// Gets all Stored Procedures of the current Database
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<DBObjectName> GetAllStoredProcedures()
        {
            string sql = "SELECT SCHEMA_NAME(schema_id) as [schema], name FROM sys.procedures ";
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            List<DBObjectName> names = new List<DBObjectName>();
            
            using (var reader = cmd.ExecuteReader())
            {
                var rh = new RowHelper(reader);
                var scOrdinal = rh.GetOrdinal("schema");
                var nameOrdinal = rh.GetOrdinal("name");
                while (reader.Read())
                {
                    
                    names.Add(new DBObjectName(rh.GetStringFieldValue(scOrdinal)!, rh.GetStringFieldValue(nameOrdinal)!));

                }
            }

            return names;
        }



        /// <summary>
        /// Get all parameter names of a Stored Procedure
        /// </summary>
        /// <returns></returns>
        public string[] GetSPParameters(DBObjectName storedProcedure, bool doNoUseCachedResults = false)
        {
            if (!doNoUseCachedResults && spParameters.TryGetValue(storedProcedure.ToString(), out string[] spPrms))
                return spPrms;
            string[]? oldValue;
            if (!spParameters.TryGetValue(storedProcedure.ToString(), out oldValue))
            {
                oldValue = null;
            }


            string command = @"SELECT PARAMETER_NAME 
FROM information_schema.parameters 
WHERE SPECIFIC_NAME = @SPName  AND SPECIFIC_SCHEMA=@Schema AND PARAMETER_NAME<>''";
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            DbCommand cmd = this.Connection.CreateCommand();
            cmd.CommandText = command;
            cmd.AddCommandParameter("@SPName", storedProcedure.Name)
                .AddCommandParameter("@Schema", storedProcedure.Schema ?? DBObjectName.DefaultSchema);
            List<string> resultL = new List<string>();

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        resultL.Add(reader.GetString(0));
                    }
                }
            }

            var result = resultL.ToArray();
            if (oldValue != null)
                spParameters.TryUpdate(storedProcedure.ToString(), result, oldValue);
            else
            {
                spParameters.TryAdd(storedProcedure.ToString(), result);
            }
            return result;
        }

        /// <summary>
        /// Get all parameter names of a Stored Procedure
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetSPParametersAsync(DBObjectName storedProcedure, bool doNoUseCachedResults = false)
        {
            if (!doNoUseCachedResults && spParameters.TryGetValue(storedProcedure.ToString(), out string[] spPrms))
                return spPrms;
            string[]? oldValue;
            if (!spParameters.TryGetValue(storedProcedure.ToString(), out oldValue))
            {
                oldValue = null;
            }


            string command = @"SELECT PARAMETER_NAME 
FROM information_schema.parameters 
WHERE SPECIFIC_NAME = @SPName  AND SPECIFIC_SCHEMA=@Schema AND PARAMETER_NAME<>''";
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            DbCommand cmd = this.Connection.CreateCommand();
            cmd.CommandText = command;
            cmd.AddCommandParameter("@SPName", storedProcedure.Name)
                .AddCommandParameter("@Schema", storedProcedure.Schema ?? DBObjectName.DefaultSchema);
            List<string> resultL = new List<string>();

            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        resultL.Add(reader.GetString(0));
                    }
                }
            }

            var result = resultL.ToArray();
            if (oldValue != null)
                spParameters.TryUpdate(storedProcedure.ToString(), result, oldValue);
            else
            {
                spParameters.TryAdd(storedProcedure.ToString(), result);
            }
            return result;
        }

        /// <summary>
        /// This method must be tested. It should create a Stored Procedure if it does not exist yet
        /// </summary>
        /// <param name="cmd"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static void CreateProcedureIfNotExists(DbCommand cmd)
        {//TODO: Testing

            var dbi = new DatabaseInformation(cmd.Connection);
            if (!dbi.GetAllStoredProcedures().Contains(cmd.CommandText))
            {
                var parametersString = new StringBuilder();
                bool first = true;
                foreach (DbParameter param in cmd.Parameters)
                {
                    if (first)
                        first = false;
                    else
                        parametersString.Append(Environment.NewLine + ",");
                    parametersString.Append("@" + param.ParameterName + " ");
                    string dbType = "nvarchar(MAX)";
                    if (param.DbType == DbType.Binary)
                    {
                        dbType = "varbinary(MAX)";
                    }
                    else if (param.DbType == DbType.Boolean)
                    {
                        dbType = "bit";
                    }
                    else if (param.DbType == DbType.Byte)
                    {
                        dbType = "tinyint";
                    }
                    else if (param.DbType == DbType.DateTime)
                    {
                        dbType = "DateTime";
                    }
                    else if (param.DbType == DbType.Int32)
                    {
                        dbType = "int";
                    }
                    else if (param.DbType == DbType.Double)
                    {
                        dbType = "float";
                    }
                    else if (param.DbType == DbType.Single)
                    {
                        dbType = "float";
                    }
                    parametersString.Append(dbType);
                }
                var template =
@"CREATE PROCEDURE {{Name}} 
    {{Parameters}}
AS
BEGIN
    PRINT 'test';
END";
                var sql = template.Replace("{{Name}}", cmd.CommandText).Replace("{{Parameters}}", parametersString.ToString());
                var execCmd = cmd.Connection.CreateCommand();

                execCmd.CommandText = sql;
                if (execCmd.Connection.State == ConnectionState.Closed)
                    execCmd.Connection.Open();
                execCmd.ExecuteNonQuery();
            }
        }
    }
}
