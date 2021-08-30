using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kull.Data
{
    public static partial class DatabaseUtils
    {

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
            IReadOnlyDictionary<string, object>? objectCol = null;
            if (entity is IReadOnlyDictionary<string, object> oc1)
            {
                objectCol = oc1;
                keys = objectCol.Keys.ToArray();
            }
            else if (entity is IReadOnlyDictionary<string, string> strdict)
            {
                objectCol = strdict.ToDictionary(k => k.Key, k => (object)k.Value);
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
                string[] parameters = GetSPParameters(cmd.CommandText, cmd.Connection!, false);
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
                    var value = objectCol == null ? properties!.First(s => s.Name == property).GetValue(entity, null) : objectCol[property];
                    AddCommandParameter(cmd, parameterPrefix + property, value);
                }
            }

            return cmd;
        }

        /// <summary>
        /// Simply adds a command parameter to a stored procedure. You can chain this method
        /// </summary>
        public static DbCommand AddCommandParameter(this DbCommand cmd, string name, object? value, Type type, bool checkParameters = false,
            Action<DbParameter>? configure = null)
        {
            var schemaParam = cmd.CreateParameter();
            schemaParam.Direction = ParameterDirection.Input;
            if (!name.StartsWith("@")
                && (cmd.GetType().FullName == "System.Data.SqlClient.SqlCommand" || cmd.GetType().FullName == "Microsoft.Data.SqlClient.SqlCommand")
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

                string[] parameters = GetSPParameters(cmd.CommandText, cmd.Connection!);
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
            if (configure != null)
            {
                configure(schemaParam);
            }
            cmd.Parameters.Add(schemaParam);
            return cmd;
        }



        /// <summary>
        /// Simply adds a command parameter to a stored procedure. You can chain this method
        /// </summary>
        public static DbCommand AddCommandParameter<T>(this DbCommand cmd, string name, T value, bool checkParameters = false, Action<DbParameter>? configure = null)
        {
            return AddCommandParameter(cmd, name, value, typeof(T), checkParameters, configure);
        }

        /// <summary>
        /// Simply adds a command parameter to a stored procedure. You can chain this method
        /// </summary>
        public static DbCommand AddTableValuedParameter<T>(this DbCommand cmd, string name, IEnumerable<T> value,
            string typeName,
            bool checkParameters = false, Action<DbParameter>? configure = null)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var dt = new System.Data.DataTable();
            foreach (var p in properties)
            {
                dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
            }
            foreach (var item in value)
            {
                dt.Rows.Add(properties.Select(s =>
                {
                    var vl = s.GetValue(item);
                    if (vl == null) return DBNull.Value;
                    return vl;
                }).ToArray());
            }

            if (cmd.GetType().FullName == "System.Data.SqlClient.SqlCommand" || cmd.GetType().FullName == "Microsoft.Data.SqlClient.SqlCommand")
            {
                Action<DbParameter> conf = (DbParameter dbPrm) =>
                {
                    dbPrm.GetType().GetProperty("TypeName").SetValue(dbPrm, typeName);
                    dbPrm.GetType().GetProperty("SqlDbType", System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.SetProperty)!
                    .SetValue(dbPrm, System.Data.SqlDbType.Structured);
                    if (configure != null)
                    {
                        configure(dbPrm);
                    }
                };
                return AddCommandParameter(cmd, name, dt, typeof(System.Data.DataTable), checkParameters, conf);
            }
            return AddCommandParameter(cmd, name, dt, typeof(System.Data.DataTable), checkParameters, configure);
        }
    }
}
