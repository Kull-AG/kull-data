using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
#if !NET2
using System.Threading.Tasks;
#endif
using dt = System.Data;
using dba = System.Data.Common;
#if NETSTD
using Microsoft.Extensions.Configuration;
#endif

namespace Kull.Data
{
    /// <summary>
    /// Class with multiple utilities for Stored Procedures and more.
    /// This class is Cross-DB (using DbProviderFactory)
    /// </summary>
    public static class DatabaseUtils
    {

        /// <summary>
        /// Synchronously checks the connection to be open and if not open, opens it
        /// </summary>
        /// <param name="con">The connection</param>
        /// <returns>The same connection</returns>
        public static DbConnection AssureOpen(this DbConnection con)
        {
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            return con;
        }


        /// <summary>
        /// Gets Parameters from procedure
        /// Calls the Database
        /// </summary>
        public static string[] GetSPParameters(DBObjectName storedProcedure, DbConnection connection, bool doNoUseCachedResults = false)
        {
            return DatabaseInformation.Create(connection).GetSPParameters(storedProcedure, doNoUseCachedResults);
        }

#if !NET2
        /// <summary>
        /// Gets Parameters from procedure
        /// Calls the Database
        /// </summary>
        public static Task<string[]> GetSPParametersAsync(DBObjectName storedProcedure, DbConnection connection, bool doNoUseCachedResults = false)
        {
            return DatabaseInformation.Create(connection).GetSPParametersAsync(storedProcedure, doNoUseCachedResults);
        }
#endif

        /// <summary>
        /// Gets the names of all parameters of an sp and sets those whose have a property in an 
        /// entity with the same name
        /// </summary>
        /// <typeparam name="T">The type of the entity</typeparam>
        /// <example>cmd.AddParametersFromEntity(new  { id=1, NTLogin= GetUserName() })</example>
        /// <param name="cmd">The command</param>
        /// <param name="entity">The entity to search for</param>
        /// <param name="checkParameters">If this is set to false, if will no be checked wheter an SP contains such a parameter. If cmd.CommandType is text, it will be set to false automatically</param>
        /// <returns>Returns the command to enable chaining</returns>
        public static DbCommand AddParametersFromEntity<T>(this DbCommand cmd,
            T entity, bool checkParameters = true)
        {
            if (entity == null)
                return cmd;//Noting to add
            if (cmd.CommandType != CommandType.StoredProcedure)
                checkParameters = false;
            string parameterPrefix = "@";

            string[] keys;
            PropertyInfo[]? properties = null;
            IDictionary<string, object>? objectCol = null;
            if (entity is IDictionary<string, object>)
            {
                objectCol = (IDictionary<string, object>)entity;
                keys = objectCol.Keys.ToArray();
            }
            else if (entity is IDictionary<string, string>)
            {
                var dict = ((IDictionary<string, string>)entity);
                objectCol = new Dictionary<string, object>();
                foreach (var item in dict)
                    objectCol.Add(item.Key, item.Value);
                keys = objectCol.Keys.ToArray();
            }
            else
            {
                var tType = typeof(T);
                properties = (tType == typeof(object) ? entity.GetType() : tType)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);
                keys = properties.Select(s => s.Name).ToArray();
            }


            if (keys.Length == 0)
                return cmd;//Nothing to add


            if (checkParameters)
            {
                string[] parameters = GetSPParameters(cmd.CommandText, cmd.Connection, false);
                foreach (var parameter in parameters)
                {
                    foreach (var property in keys)
                    {
                        if (string.Equals(parameter, parameterPrefix + property, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var prop = properties?.First(s => s.Name == property);
                            var value = objectCol == null ? prop!.GetValue(entity, null) : objectCol[property];
                            AddCommandParameter(cmd, parameter,
                                value);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var property in keys)
                {
                    var value = objectCol == null ? properties.First(s => s.Name == property).GetValue(entity, null) : objectCol[property];
                    AddCommandParameter(cmd, parameterPrefix + property, value);
                }
            }

            return cmd;
        }

        /// <summary>
        /// Simply adds a command parameter to a stored procedure. You can chain this method
        /// </summary>
        public static DbCommand AddCommandParameter(this DbCommand cmd, string name, object? value, Type type, bool checkParameters = false)
        {
            var schemaParam = cmd.CreateParameter();
            schemaParam.Direction = ParameterDirection.Input;
            if (!name.StartsWith("@")
#if NETSTD
                && (cmd.GetType().FullName == "System.Data.SqlClient.SqlCommand")
#else
                && (cmd is System.Data.SqlClient.SqlCommand)
#endif
                )
            {
                name = "@" + name;
            }
            if (cmd.Parameters.Contains(name))
            {
                cmd.Parameters.Remove(cmd.Parameters[name]);
            }
            if (cmd.CommandType != CommandType.StoredProcedure)
                checkParameters = false;
            if (checkParameters)
            {

                string[] parameters = GetSPParameters(cmd.CommandText, cmd.Connection);
                if (!parameters.Contains(name, StringComparer.CurrentCultureIgnoreCase))
                    return cmd;
            }
            schemaParam.ParameterName = name;
            schemaParam.Value = (value as object) ?? DBNull.Value;
            // Todo: Use a  map for this
            if (type == typeof(byte[]))
            {
                schemaParam.DbType = DbType.Binary;
            }
            cmd.Parameters.Add(schemaParam);
            return cmd;
        }

        /// <summary>
        /// Simply adds a command parameter to a stored procedure. You can chain this method
        /// </summary>
        public static DbCommand AddCommandParameter<T>(this DbCommand cmd, string name, T value, bool checkParameters = false)
        {
            return AddCommandParameter(cmd, name, value, typeof(T), checkParameters);
        }

        /// <summary>
        /// Gets a Data Adapter for a command
        /// </summary>
        [Obsolete("Use Datareader instead")]
        public static dba.DbDataAdapter CreateAdapter(this DbCommand cmd)
        {
#if NETSTD2
            if (cmd.GetType().FullName == "System.Data.SqlClient.SqlCommand")
            {
                return (DbDataAdapter)Activator.CreateInstance(Type.GetType("System.Data.SqlClient.SqlDataAdapter, System.Data.SqlClient", true), cmd);
            }
            else if (cmd.GetType().FullName == "System.Data.SqlClient.OleDbCommand")
            {
                return (DbDataAdapter)Activator.CreateInstance(Type.GetType("System.Data.SqlClient.OleDbCommand, System.Data.SqlClient", true), cmd);
            }
            else if (cmd.GetType().FullName == "System.Data.SqlClient.OdbcCommand")
            {
                return (DbDataAdapter)Activator.CreateInstance(Type.GetType("System.Data.SqlClient.OdbcDataAdapter, System.Data.SqlClient", true), cmd);
            }
            throw new NotSupportedException("CommandType not supported. Create adapter manually");

#elif NET2

            if (cmd is System.Data.SqlClient.SqlCommand)
            {
                return new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)cmd);

            }
            else if (cmd is System.Data.OleDb.OleDbCommand)
            {
                return new System.Data.OleDb.OleDbDataAdapter((System.Data.OleDb.OleDbCommand)cmd);

            }
            else if (cmd is System.Data.Odbc.OdbcCommand)
            {
                return new System.Data.Odbc.OdbcDataAdapter((System.Data.Odbc.OdbcCommand)cmd);

            }
            throw new NotSupportedException("CommandType not supported. Create adapter manually");
#else
            var factory = dba.DbProviderFactories.GetFactory(cmd.Connection);
            var adapt = factory.CreateDataAdapter();
            adapt.SelectCommand = cmd;
            return adapt;
#endif
        }

        /// <summary>
        /// Return the Table return from the Stored Procedure
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Obsolete("Use DbDataReader instead")]
        public static dt.DataTable ReadAsTable(this DbCommand cmd)
        {
            var adapt = CreateAdapter(cmd);
            dt.DataTable dt = new dt.DataTable();
            adapt.Fill(dt);
            return dt;
        }
#if !NET2
        /// <summary>
        /// Returns asynchronously the Table of the Command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Obsolete("Use DbDataReader instead")]
        public static async Task<dt.DataTable> ReadAsTableAsync(this DbCommand cmd)
        {
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                await cmd.Connection.OpenAsync().ConfigureAwait(false);
            }
            using (var dataReader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                dt.DataTable dt = new dt.DataTable();
                dt.Load(dataReader);
                return dt;
            }
        }
#endif
        /// <summary>
        /// This may be useful if you want to serialize a full table as json. Usually, a IDictionary object is serialized as JSON object (eg Newtonsoft.Json)
        /// It can be used with dynmaic as well (http://stackoverflow.com/questions/18290852/generate-dynamic-object-from-dictionary-with-c-reflection)
        /// </summary>
        /// <typeparam name="T">The type of the IDictionary, usually Dictionary&lt;string,object&gt;</typeparam>
        /// <param name="table">The datatable to convert to a Dictionary List</param>
        /// <param name="inputList">An existing list to append the items. Can be null to create a new list</param>
        /// <returns>A List containing the Objects</returns>
#if !NET2
        [System.Diagnostics.Contracts.Pure]
#endif
        [Obsolete("Use DbDataReader instead")]
        public static List<T> AsDictionaryList<T>(this dt.DataTable table, List<T>? inputList = null) where T : IDictionary<string, object>, new()
        {
            if (inputList == null)
            {
                inputList = new List<T>();
            }
            var columns = table.Columns.Cast<dt.DataColumn>().ToArray();

            foreach (dt.DataRow row in table.Rows)
            {
                T item = new T();
                foreach (var col in columns)
                {
                    item.Add(col.ColumnName, row[col]);
                }
                inputList.Add(item);
            }
            return inputList;
        }
        /// <summary>
        /// This may be useful if you want to serialize a full table as json. Usually, a IDictionary object is serialized as JSON object (eg Newtonsoft.Json)
        /// It can be used with dynmaic as well (http://stackoverflow.com/questions/18290852/generate-dynamic-object-from-dictionary-with-c-reflection)
        /// </summary>
        /// <typeparam name="T">The type of the IDictionary, usually Dictionary&lt;string,object&gt;</typeparam>
        /// <param name="table">The DbDataReader to convert to a Dictionary List</param>
        /// <param name="inputList">An existing list to append the items. Can be null to create a new list</param>
        /// <returns>A List containing the Objects</returns>
        public static List<T> AsDictionaryList<T>(this System.Data.Common.DbDataReader table, List<T>? inputList = null) where T : IDictionary<string, object?>, new()
        {
            if (inputList == null)
            {
                inputList = new List<T>();
            }

            int columnCount = table.FieldCount;



            while (table.Read())
            {
                T item = new T();
                foreach (var col in Enumerable.Range(0, columnCount))
                {
                    item.Add(table.GetName(col), table.IsDBNull(col) ? null : table[col]);
                }
                inputList.Add(item);
            }
            return inputList;
        }

        /// <summary>
        /// Returns all Tables returned from the Procedure
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Obsolete("Use DbDataReader instead")]
        public static dt.DataSet ReadAsDataSet(this DbCommand cmd)
        {

            var adapt = CreateAdapter(cmd);
            dt.DataSet dt = new dt.DataSet();
            adapt.Fill(dt);
            return dt;
        }
#if !NET2
        /// <summary>
        /// Returns all Tables returned from the Procedure
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Obsolete("Use DbDataReader instead")]
        public static async Task<dt.DataSet> ReadAsDataSetAsync(this DbCommand cmd)
        {
            var adapt = CreateAdapter(cmd);
            return await Task.Run(() =>
            {//Not a nice implementation but the only one that is currently possible
                dt.DataSet dt = new dt.DataSet();
                adapt.Fill(dt);
                return dt;
            }).ConfigureAwait(false);
        }
#endif
        /// <summary>
        /// Returns a DbCommand Object for a Procedure
        /// </summary>
        /// <param name="con"></param>
        /// <param name="text">The text to execute</param>
        /// <param name="commandType">The type of the command</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DbCommand CreateCommand(this DbConnection con, string text, CommandType commandType)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = commandType;
            return cmd;
        }


        /// <summary>
        /// Returns a DbCommand Object for a Procedure
        /// </summary>
        /// <param name="con"></param>
        /// <param name="nameOfStoredProcedure"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DbCommand CreateSPCommand(this DbConnection con, DBObjectName nameOfStoredProcedure)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = nameOfStoredProcedure.ToString(false);
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        /// <summary>
        /// Returns a DbCommand Object for a Procedure
        /// </summary>
        /// <param name="con"></param>
        /// <param name="nameOfStoredProcedure"></param>
        /// <returns></returns>
        [Obsolete("Use CreateSPCommand instead")]
        public static DbCommand CreateSP(this DbConnection con, DBObjectName nameOfStoredProcedure)
            => CreateSPCommand(con, nameOfStoredProcedure);


        /// <summary>
        /// Executes a stored function on the database. Parameters do not have to be in correct order, but they should
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        /// <typeparam name="TParam">The type of the Parameters</typeparam>
        /// <param name="con">The Connection</param>
        /// <param name="nameOfFunction">The name of the function</param>
        /// <param name="parameters">An object containing parameters (in key/value form)</param>
        /// <param name="checkParameters">Set this to false if you do not want this function to make an extra select on the db to get
        /// the parameters of the stored function. If you set this to false, ther parameters must be in correct order</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static T ExecuteScalarFunction<T, TParam>(this DbConnection con, DBObjectName nameOfFunction, TParam parameters,
            bool checkParameters = true)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = nameOfFunction.ToString(false);//We have to set this in order to make the next line work with checkPArameters
            cmd.AddParametersFromEntity<TParam>(parameters, checkParameters);
            cmd.CommandType = CommandType.Text;

            string[] sparams;
            if (checkParameters)
            {
                sparams = GetSPParameters(nameOfFunction, con);
                if (cmd.Parameters.Count != sparams.Length)
                {//We have to set default values
                    string[] existingParameters = cmd.Parameters.OfType<DbParameter>().Select(s => s.ParameterName.ToLower()).ToArray();
                    foreach (var sparam in sparams)
                    {
                        if (!existingParameters.Contains(sparam.ToLower()))
                        {
                            cmd.AddCommandParameter(sparam, DBNull.Value);
                        }
                    }
                }
            }
            else
                sparams = cmd.Parameters.OfType<DbParameter>().Select(s => s.ParameterName).ToArray();



            cmd.CommandText = "SELECT " + nameOfFunction.ToString(false) + "(" + string.Join(",", sparams) + ")";
            using (var tbl = cmd.ExecuteReader())
            {
                tbl.Read();
                var vl = tbl.GetValue(0);
                if (vl is T)
                    return (T)vl;
                else
                {
                    return (T)vl;//Throw
                }
            }

        }

#if !NET2
        /// <summary>
        /// Executes a stored function on the database. Parameters do not have to be in correct order, but they should
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        /// <typeparam name="TParam">The type of the Parameters</typeparam>
        /// <param name="con">The Connection</param>
        /// <param name="nameOfFunction">The name of the function</param>
        /// <param name="parameters">An object containing parameters (in key/value form)</param>
        /// <param name="checkParameters">Set this to false if you do not want this function to make an extra select on the db to get
        /// the parameters of the stored function. If you set this to false, ther parameters must be in correct order</param>
        public static async Task<T> ExecuteScalarFunctionAsync<T, TParam>(this DbConnection con, DBObjectName nameOfFunction, TParam parameters,
            bool checkParameters = true)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = nameOfFunction.ToString(false);//We have to set this in order to make the next line work with checkPArameters
            cmd.AddParametersFromEntity<TParam>(parameters, checkParameters);
            cmd.CommandType = CommandType.Text;

            string[] sparams;
            if (checkParameters)
            {
                sparams = GetSPParameters(nameOfFunction, con);
                if (cmd.Parameters.Count != sparams.Length)
                {//We have to set default values
                    string[] existingParameters = cmd.Parameters.OfType<DbParameter>().Select(s => s.ParameterName.ToLower()).ToArray();
                    foreach (var sparam in sparams)
                    {
                        if (!existingParameters.Contains(sparam.ToLower()))
                        {
                            cmd.AddCommandParameter(sparam, DBNull.Value);
                        }
                    }
                }
            }
            else
                sparams = cmd.Parameters.OfType<DbParameter>().Select(s => s.ParameterName).ToArray();



            cmd.CommandText = "SELECT " + nameOfFunction.ToString(false) + "(" + string.Join(",", sparams) + ")";
            using (var tbl = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                tbl.Read();
                var vl = tbl.GetValue(0);
                if (vl is T)
                    return (T)vl;
                else
                {
                    return (T)vl;//Throw
                }
            }

        }
#endif
        /// <summary>
        /// Gets a connection from the provided Entity Framework Style Connection String.
        /// Attention: This only works with providers other then SQL Server in .Net Standard 2.1+ and in full .Net Framework
        /// </summary>
        /// <param name="entityFrameworkConnectionString">The Connection string</param>
        /// <param name="defaultProviderName">The Provider name if no provider is specified</param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromEFString(string entityFrameworkConnectionString, string? defaultProviderName)
        {

            //Parse the connection string
            var connStrEF = EFFallback.ConnectionStringParser.ParseEF(entityFrameworkConnectionString);
            if (connStrEF.Provider == null && defaultProviderName == null)
            {
                throw new ArgumentException("Must provide a correct EF Connection string");
            }
#if !NETSTD2
            var factory = DbProviderFactories.GetFactory(connStrEF.Provider ?? defaultProviderName);//Gets the correct provider (usually System.Data.SqlClient.SqlClientFactory)
            var connection = factory.CreateConnection();
            connection.ConnectionString = connStrEF.ConnectionString;
            return connection;
#else 
            string? provider = connStrEF.Provider ?? defaultProviderName;
            if (provider == "System.Data.SqlClient")
            {
                return (DbConnection)Activator.CreateInstance(Type.GetType("System.Data.SqlClient.SqlConnection, System.Data.SqlClient", true), connStrEF.ConnectionString);
            }
            else
            {
                throw new NotSupportedException("On .Net STD / .Net Core 2.0 currently only SQL Server is supported. Use .Net Standard 2.1 or full .Net Fx if possible");
            }
#endif

        }

        /// <summary>
        /// Gets a Connection from a Entity Framework Connection String
        /// </summary>
        /// <param name="entityFrameworkConnectionString"></param>
        /// <param name="checkEf"></param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromEFString(string entityFrameworkConnectionString,
            bool checkEf = false)
        {
#if NETSTD2
            if (checkEf && !entityFrameworkConnectionString.Contains("provider="))
                return (DbConnection)Activator.CreateInstance(Type.GetType("System.Data.SqlClient.SqlConnection, System.Data.SqlClient", true), entityFrameworkConnectionString);

            var connStrEF = EFFallback.ConnectionStringParser.ParseEF(entityFrameworkConnectionString);
            if (connStrEF.Provider == "System.Data.SqlClient.SqlClientFactory" || connStrEF.Provider == "System.Data.SqlClient")
                return (DbConnection)Activator.CreateInstance(Type.GetType("System.Data.SqlClient.SqlConnection, System.Data.SqlClient", true), connStrEF.ConnectionString);
            throw new NotSupportedException("Connection Type not supported");
#else
            string defaultProvider = "System.Data.SqlClient";
            return GetConnectionFromEFString(entityFrameworkConnectionString, checkEf ? defaultProvider : null);
#endif
        }

        /// <summary>
        /// Reads a connection string for environment variables. Supported are:
        /// - The name of the connection itself
        /// - SQLCONNSTR_ / SQLAZURECONNSTR_ for MSSQL
        /// - MYSQLCONNSTR_ for Mysql
        /// - PostgreSQLCONNSTR_ for PostgreSQL
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromEnvironment(string configName)
        {
            var nameTypeMap = new Dictionary<string, string>()
           {
               { "", "System.Data.SqlClient" },
               { "SQLCONNSTR_", "System.Data.SqlClient"},
               { "SQLAZURECONNSTR_", "System.Data.SqlClient"},
               { "MYSQLCONNSTR_", "MySql.Data.MySqlClient"},
               { "PostgreSQLCONNSTR_", "Npgsql"},
               { "CUSTOMCONNSTR_", "System.Data.SqlClient"}
            };
            foreach (var item in nameTypeMap)
            {
                string val = Environment.GetEnvironmentVariable(item.Key + configName);
                if (val != null)
                {
                    return GetConnectionFromEFString(val, item.Value);
                }
            }

            throw new ArgumentException("Cannot find setting " + configName);
        }

#if !NETSTD
        /// <summary>
        /// Gets a connection from a ConfigName. This will search in System.Configuration.ConfigurationManager.ConnectionStrings
        /// and System.Configuration.ConfigurationManager.AppSettings for any valid Entity Framework or Sql Server connection string
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromConfig(string configName)
        {

            if (System.Configuration.ConfigurationManager.ConnectionStrings[configName] != null)
            {
                return GetConnectionFromEFString(System.Configuration.ConfigurationManager.ConnectionStrings[configName].ConnectionString, true);
            }
            else if (System.Configuration.ConfigurationManager.AppSettings[configName] != null)
            {
                return GetConnectionFromEFString(System.Configuration.ConfigurationManager.AppSettings[configName], true);
            }
            return GetConnectionFromEnvironment(configName);
        }
#else
        /// <summary>
        /// Gets the Connection from the appsettings.json file
        /// </summary>
        /// <param name="configName">The key in the file, in the ConnectionStrings section</param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromConfig(string configName)
        {

            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, false);


            var Configuration = builder.Build();
            var value = Configuration["ConnectionStrings:" + configName];
            if (string.IsNullOrEmpty(value))
            {
                return GetConnectionFromEnvironment(configName);
            }
            return GetConnectionFromEFString(value, true);
        }
#endif

        /// <summary>
        /// Returns a list (fully loaded in RAM) of the DB Objects
        /// </summary>
        /// <typeparam name="T">The Type to return</typeparam>
        /// <param name="cmd">The command</param>
        /// <param name="ignoreMissingColumns">True to not throw if the class has more properties then the result set</param>
        /// <returns>A List of the items</returns>
        public static T[] AsArrayOf<T>(this DbCommand cmd, bool ignoreMissingColumns = false)
            where T : class, new()
        {
            cmd.Connection.AssureOpen();
            using (var rdr = cmd.ExecuteReader())
            {
                return Kull.Data.RowHelper.FromTable<T>(rdr, ignoreMissingColumns);
            }
        }
#if !NET2

        /// <summary>
        /// Returns a list (fully loaded in RAM) of the DB Objects
        /// </summary>
        /// <typeparam name="T">The Type to return</typeparam>
        /// <param name="cmd">The command</param>
        /// <param name="ignoreMissingColumns">True to not throw if the class has more properties then the result set</param>
        /// <returns>A List of the items</returns>
        public static async Task<T[]> AsArrayOfAsync<T>(this DbCommand cmd, bool ignoreMissingColumns = false)
            where T : class, new()
        {
            cmd.Connection.AssureOpen();
            using (var rdr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                return Kull.Data.RowHelper.FromTable<T>(rdr, ignoreMissingColumns);
            }
        }
#endif
    }
}
