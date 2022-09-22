using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dt = System.Data;
using dba = System.Data.Common;
using System.Diagnostics.CodeAnalysis;
#if NETSTD || NETCOREAPP
using Microsoft.Extensions.Configuration;
#endif

namespace Kull.Data
{
    /// <summary>
    /// Class with multiple utilities for Stored Procedures and more.
    /// This class is Cross-DB (using DbProviderFactory)
    /// </summary>
    public static partial class DatabaseUtils
    {
        /// <summary>
        /// Set this to true if you want to use Microsoft.Data.SqlClient for MSSQL
        /// </summary>
        public static bool UseNewMSSqlClient { get; set; } = true;

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
        /// Asynchronously checks the connection to be open and if not open, opens it
        /// </summary>
        /// <param name="con">The connection</param>
        /// <returns>The same connection</returns>
        public static async Task<DbConnection> AssureOpenAsync(this DbConnection con)
        {
            if (con.State != ConnectionState.Open)
            {
                await con.OpenAsync().ConfigureAwait(false);
            }
            return con;
        }

        /// <summary>
        /// Asynchronously checks the connection to be open and if not open, opens it
        /// </summary>
        /// <param name="con">The connection</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The same connection</returns>
        public static async Task<DbConnection> AssureOpenAsync(this DbConnection con, System.Threading.CancellationToken cancellationToken)
        {
            if (con.State != ConnectionState.Open)
            {
                await con.OpenAsync(cancellationToken).ConfigureAwait(false);
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

        /// <summary>
        /// Gets Parameters from procedure
        /// Calls the Database
        /// </summary>
        public static Task<string[]> GetSPParametersAsync(DBObjectName storedProcedure, DbConnection connection, bool doNoUseCachedResults = false)
        {
            return DatabaseInformation.Create(connection).GetSPParametersAsync(storedProcedure, doNoUseCachedResults);
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
            else if (cmd.GetType().FullName == "Microsoft.Data.SqlClient.SqlCommand")
            {
                return (DbDataAdapter)Activator.CreateInstance(Type.GetType("Microsoft.Data.SqlClient.SqlDataAdapter, Microsoft.Data.SqlClient", true), cmd);
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
#else
            var factory = dba.DbProviderFactories.GetFactory(cmd.Connection!);
            var adapt = factory!.CreateDataAdapter();
            adapt!.SelectCommand = cmd;
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

        /// <summary>
        /// Returns asynchronously the Table of the Command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Obsolete("Use DbDataReader instead")]
#if NET6_0_OR_GREATER
        [RequiresUnreferencedCode("Members from types used in the expression column to be trimmed if not referenced directly")]
#endif
        public static async Task<dt.DataTable> ReadAsTableAsync(this DbCommand cmd)
        {
            if (cmd.Connection!.State == ConnectionState.Closed)
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

        /// <summary>
        /// This may be useful if you want to serialize a full table as json. Usually, a IDictionary object is serialized as JSON object (eg Newtonsoft.Json)
        /// It can be used with dynmaic as well (http://stackoverflow.com/questions/18290852/generate-dynamic-object-from-dictionary-with-c-reflection)
        /// </summary>
        /// <typeparam name="T">The type of the IDictionary, usually Dictionary&lt;string,object&gt;</typeparam>
        /// <param name="table">The datatable to convert to a Dictionary List</param>
        /// <param name="inputList">An existing list to append the items. Can be null to create a new list</param>
        /// <returns>A List containing the Objects</returns>
        [System.Diagnostics.Contracts.Pure]
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

        /// <summary>
        /// Returns a DbCommand Object for a Procedure
        /// </summary>
        /// <param name="con"></param>
        /// <param name="text">The text to execute</param>
        /// <param name="commandType">The type of the command</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DbCommand CreateCommand(this DbConnection con, string text, CommandType commandType = CommandType.Text)
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
            cmd.CommandText = nameOfStoredProcedure.ToString(false, true);
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
        public static T ExecuteScalarFunction<T, TParam>(this DbConnection con, DBObjectName nameOfFunction, TParam parameters,
            bool checkParameters = true)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = nameOfFunction.ToString(false, true);//We have to set this in order to make the next line work with checkPArameters
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



            cmd.CommandText = "SELECT " + nameOfFunction.ToString(false, true) + "(" + string.Join(",", sparams) + ")";
            using (var tbl = cmd.ExecuteReader())
            {
                tbl.Read();
                var vl = tbl.IsDBNull(0) ? default(T) : tbl.GetValue(0);
                if (vl is T)
                    return (T)vl;
                else
                {
                    return (T)Convert.ChangeType(vl, typeof(T));//Throw
                }
            }

        }


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
            cmd.CommandText = nameOfFunction.ToString(false, true);//We have to set this in order to make the next line work with checkPArameters
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



            cmd.CommandText = "SELECT " + nameOfFunction.ToString(false, true) + "(" + string.Join(",", sparams) + ")";
            using (var tbl = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                tbl.Read();
                var vl = tbl.IsDBNull(0) ? default(T) : tbl.GetValue(0);
                if (vl is T)
                    return (T)vl;
                else
                {
                    return (T)Convert.ChangeType(vl, typeof(T));//Throw
                }
            }

        }

#if !NETSTD2
        /// <summary>
        /// Gets a connection from the provided Entity Framework Style Connection String.
        /// Attention: This only works with providers other then SQL Server in .Net Standard 2.1+ and in full .Net Framework
        /// </summary>
        /// <param name="entityFrameworkConnectionString">The Connection string</param>
        /// <param name="defaultProvider">The Provider if no provider is specified</param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromEFString(string entityFrameworkConnectionString, DbProviderFactory defaultProvider)
        {
            var connStrEF = EFFallback.ConnectionStringParser.ParseEF(entityFrameworkConnectionString);
            if (connStrEF.Provider == null && defaultProvider == null)
            {
                throw new ArgumentException("Must provide a correct EF Connection string");
            }
            var factory = connStrEF.Provider != null ? DbProviderFactories.GetFactory(connStrEF.Provider) : defaultProvider;//Gets the correct provider (usually System.Data.SqlClient.SqlClientFactory)
            var connection = factory.CreateConnection();
            connection!.ConnectionString = connStrEF.ConnectionString;
            return connection;
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
            var factory = DbProviderFactories.GetFactory(connStrEF.Provider ?? defaultProviderName!);//Gets the correct provider (usually System.Data.SqlClient.SqlClientFactory)
            var connection = factory.CreateConnection();
            connection!.ConnectionString = connStrEF.ConnectionString;
            return connection;
#else 
            string? provider = connStrEF.Provider ?? defaultProviderName;
            if (provider == "System.Data.SqlClient")
            {
                return (DbConnection)Activator.CreateInstance(Type.GetType("System.Data.SqlClient.SqlConnection, System.Data.SqlClient", true), connStrEF.ConnectionString);
            }
            else if (provider == "Microsoft.Data.SqlClient")
            {
                return (DbConnection)Activator.CreateInstance(Type.GetType("Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient", true), connStrEF.ConnectionString);
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
            if (connStrEF.Provider == "Microsoft.Data.SqlClient.SqlClientFactory" || connStrEF.Provider == "Microsoft.Data.SqlClient")
                return (DbConnection)Activator.CreateInstance(Type.GetType("Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient", true), connStrEF.ConnectionString);
            throw new NotSupportedException("Connection Type not supported");
#else
            string defaultProvider = UseNewMSSqlClient ? "Microsoft.Data.SqlClient" : "System.Data.SqlClient";
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
        /// <param name="throwOnNotFound">true to throw ArgumentException instead of returning null</param>
        /// <param name="defaultFactory">If not stated in connection string, provides the default provider. Requires .Net Framework or .Net Standard 2.1+</param>
        /// <returns></returns>
        public static DbConnection? GetConnectionFromEnvironment(string configName, bool throwOnNotFound, DbProviderFactory? defaultFactory)

        {
            var msFactory = UseNewMSSqlClient ? "Microsoft.Data.SqlClient" : "System.Data.SqlClient";
            var nameTypeMap = new Dictionary<string, string?>()
           {
               { "",  null},
               { "SQLCONNSTR_",  msFactory},
               { "SQLAZURECONNSTR_", msFactory},
               { "MYSQLCONNSTR_", "MySql.Data.MySqlClient"},
               { "PostgreSQLCONNSTR_", "Npgsql"},
               { "CUSTOMCONNSTR_",  null}
            };
            foreach (var item in nameTypeMap)
            {
                string? val = Environment.GetEnvironmentVariable(item.Key + configName);
                if (val != null)
                {
#if !NETSTD2
                    if (item.Value == null && defaultFactory != null)
                    {
                        return GetConnectionFromEFString(val, defaultFactory);
                    }
#endif
                    return GetConnectionFromEFString(val, item.Value ?? msFactory);
                }
            }
            if (throwOnNotFound)
            {
                throw new ArgumentException("Cannot find setting " + configName);
            }
            else
            {
                return null;
            }
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
        [Obsolete("Use GetConnectionFromEnvironment(string, bool)")]
        public static DbConnection GetConnectionFromEnvironment(string configName) => GetConnectionFromEnvironment(configName, true, null)!;

#if !NETSTD && !NETCOREAPP
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
            return GetConnectionFromEnvironment(configName, true, null)!;
        }


#else
        /// <summary>
        /// Gets the Connection from the appsettings.json file
        /// </summary>
        /// <param name="configName">The key in the file, in the ConnectionStrings section</param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromConfig(string configName)
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_Environment");

            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, false);
            if(env != null)
            {
                builder.AddJsonFile($"appsettings.{env}.json", true, false);
            }

            var Configuration = builder.Build();
            var value = Configuration["ConnectionStrings:" + configName];
            if (string.IsNullOrEmpty(value))
            {
                return GetConnectionFromEnvironment(configName, true, null)!;
            }
            return GetConnectionFromEFString(value, true);
        }
#endif

#if !NETSTD2
        /// <summary>
        /// Gets a connection from a ConfigName. This will search in System.Configuration.ConfigurationManager.ConnectionStrings
        /// and System.Configuration.ConfigurationManager.AppSettings for any valid Entity Framework or Sql Server connection string
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="defaultProviderFactory">The default factory if none is specified</param>
        /// <returns></returns>
        public static DbConnection GetConnectionFromConfig(string configName, DbProviderFactory defaultProviderFactory)
        {
#if !NETSTD && !NETCOREAPP
            if (System.Configuration.ConfigurationManager.ConnectionStrings[configName] != null)
            {
                return GetConnectionFromEFString(System.Configuration.ConfigurationManager.ConnectionStrings[configName].ConnectionString, defaultProviderFactory);
            }
            else if (System.Configuration.ConfigurationManager.AppSettings[configName] != null)
            {
                return GetConnectionFromEFString(System.Configuration.ConfigurationManager.AppSettings[configName], defaultProviderFactory);
            }
#else
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, false);

            string env = Environment.GetEnvironmentVariable("ASPNETCORE_Environment");
            if(env != null)
            {
                builder.AddJsonFile($"appsettings.{env}.json", true, false);
            }
            var Configuration = builder.Build();
            var value = Configuration["ConnectionStrings:" + configName];
            if (!string.IsNullOrEmpty(value))
            {
                return GetConnectionFromEFString(value, defaultProviderFactory);
            }
#endif

            return GetConnectionFromEnvironment(configName, true, defaultProviderFactory)!;
        }
#endif

        /// <summary>
        /// Returns a list (fully loaded in RAM) of the DB Objects
        /// </summary>
        /// <typeparam name="T">The Type to return</typeparam>
        /// <param name="cmd">The command</param>
        /// <param name="ignoreMissingColumns">True to not throw if the class has more properties then the result set</param>
        /// <returns>A List of the items</returns>
        public static IReadOnlyCollection<T> AsCollectionOf<T>(this DbCommand cmd, bool ignoreMissingColumns = false)
        where T : class
        {
            cmd.Connection!.AssureOpen();
            using (var rdr = cmd.ExecuteReader())
            {
                return Kull.Data.RowHelper.FromTable<T>(rdr, ignoreMissingColumns);
            }
        }


        /// <summary>
        /// Returns a list (fully loaded in RAM) of the DB Objects
        /// </summary>
        /// <typeparam name="T">The Type to return</typeparam>
        /// <param name="cmd">The command</param>
        /// <param name="ignoreMissingColumns">True to not throw if the class has more properties then the result set</param>
        /// <returns>A List of the items</returns>
        public static async Task<IReadOnlyCollection<T>> AsCollectionOfAsync<T>(this DbCommand cmd, bool ignoreMissingColumns = false)
            where T : class
        {
            await cmd.Connection!.AssureOpenAsync().ConfigureAwait(false);
            using (var rdr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                return Kull.Data.RowHelper.FromTable<T>(rdr, ignoreMissingColumns);
            }
        }

        /// <summary>
        /// Returns a list (fully loaded in RAM) of the DB Objects
        /// </summary>
        /// <typeparam name="T">The Type to return</typeparam>
        /// <param name="cmd">The command</param>
        /// <param name="ignoreMissingColumns">True to not throw if the class has more properties then the result set</param>
        /// <returns>A List of the items</returns>
        [Obsolete("Use AsCollectionOf which is immutable")]
        public static T[] AsArrayOf<T>(this DbCommand cmd, bool ignoreMissingColumns = false)
        where T : class
        {
            cmd.Connection!.AssureOpen();
            using (var rdr = cmd.ExecuteReader())
            {
                return Kull.Data.RowHelper.FromTable<T>(rdr, ignoreMissingColumns).ToArray();
            }
        }


        /// <summary>
        /// Returns a list (fully loaded in RAM) of the DB Objects
        /// </summary>
        /// <typeparam name="T">The Type to return</typeparam>
        /// <param name="cmd">The command</param>
        /// <param name="ignoreMissingColumns">True to not throw if the class has more properties then the result set</param>
        /// <returns>A List of the items</returns>
        [Obsolete("Use AsCollectionOfAsync which is immutable")]
        public static async Task<T[]> AsArrayOfAsync<T>(this DbCommand cmd, bool ignoreMissingColumns = false)
            where T : class
        {
            await cmd.Connection!.AssureOpenAsync().ConfigureAwait(false);
            using (var rdr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                return Kull.Data.RowHelper.FromTable<T>(rdr, ignoreMissingColumns).ToArray();
            }
        }


    }
}
