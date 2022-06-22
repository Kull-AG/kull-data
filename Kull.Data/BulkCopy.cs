using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kull.Data
{
    public static class BulkCopy
    {
        private static void GenericInsert(this DbConnection connection, DBObjectName destinationTable, DbDataReader source)
        {
            // Innspired by https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/bulk-insert
            using (var transaction = connection.BeginTransaction())
            {
                DbCommand cmd = transaction.CreateCommand("");
                string[] fieldNames = Enumerable.Range(0, source.FieldCount)
                    .Select(i => source.GetName(i)).ToArray();
                string colNamesQuoted = string.Join(", ", fieldNames.Select(s => new DBObjectName(null, s).ToString(false, true)));
                string prmNames = string.Join(", ", Enumerable.Range(0, source.FieldCount)
                    .Select(s => "@p" + s.ToString()));

                cmd.CommandText = "INSERT INTO " + destinationTable.ToString(false, true) + "(" + colNamesQuoted + ")"
                        + " VALUES (" + prmNames + ")";
                var prms = Enumerable.Range(0, source.FieldCount)
                    .Select(s =>
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@p" + s.ToString();
                        cmd.Parameters.Add(p);
                        return p;
                    }).ToArray();
                while (source.Read())
                {
                    for (int i = 0; i < prms.Length; i++)
                    {
                        prms[i].Value = source.IsDBNull(i) ? DBNull.Value : source.GetValue(i);
                    }
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Will be used for options on bulk insert. Not used currently
        /// </summary>
#pragma warning disable CA1034 // Nested types should not be visible . Can not change this for compat
        public class BulkInsertOptions
#pragma warning restore CA1034 // Nested types should not be visible
        {
        }

        public static void BulkInsert(this DbConnection connection, DBObjectName destinationTable, DbDataReader source,
            BulkInsertOptions? options = null)
        {
            Type bulkCopyType;
            Type optionsType;
            if (connection.GetType().FullName == "System.Data.SqlClient.SqlConnection")
            {
                bulkCopyType = Type.GetType("System.Data.SqlClient.SqlBulkCopy, System.Data.SqlClient", throwOnError: true)!;
                optionsType = Type.GetType("System.Data.SqlClient.SqlBulkCopyOptions, System.Data.SqlClient", throwOnError: true)!;
            }
            else if (connection.GetType().FullName == "Microsoft.Data.SqlClient.SqlConnection")
            {
                bulkCopyType = Type.GetType("Microsoft.Data.SqlClient.SqlBulkCopy, Microsoft.Data.SqlClient", throwOnError: true)!;
                optionsType = Type.GetType("Microsoft.Data.SqlClient.SqlBulkCopyOptions, Microsoft.Data.SqlClient", throwOnError: true)!;
            }
            else
            {
                GenericInsert(connection, destinationTable, source);
                return;
            }
            var tableLockValue = Convert.ChangeType(0x4, optionsType);
            var cp = Activator.CreateInstance(bulkCopyType, connection, tableLockValue, null);
            cp.GetType().GetProperty("DestinationTableName", System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.SetProperty)!.SetValue(cp, destinationTable.ToString(false, true));
            var colMappings = cp.GetType().GetProperty("ColumnMappings", System.Reflection.BindingFlags.Public |
                   System.Reflection.BindingFlags.Instance |
                   System.Reflection.BindingFlags.SetProperty)!;
            var addMethod = colMappings.GetType().GetMethod("Add", new Type[] { typeof(string), typeof(string) })!;

            string colInfos = "";
            for (int i = 0; i < source.FieldCount; i++)
            {
                string columnName = source.GetName(i);
                colInfos += columnName + ": " + source.GetDataTypeName(i) + "  " + source.GetFieldType(i) + Environment.NewLine;
                addMethod.Invoke(colMappings, new object[] { source.GetName(i), columnName });

            }
            cp.GetType().GetMethod("WriteToServer", new Type[] { typeof(DbDataReader) })!.Invoke(cp, new object[] { source });
            /*
            var cp3 = new System.Data.SqlClient.SqlBulkCopy((Microsoft.Data.SqlClient.SqlConnection)connection,
                System.Data.SqlClient.SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = destinationTable
            };

            string colInfos = "";
            for (int i = 0; i < source.FieldCount; i++)
            {
                string columnName = source.GetName(i);
                colInfos += columnName + ": " + source.GetDataTypeName(i) + "  " + source.GetFieldType(i) + Environment.NewLine;
                cp3.ColumnMappings.Add(source.GetName(i), columnName);
            }
            cp.NotifyAfter = 100;
            try
            {
                cp3.WriteToServer(source);
            }
            catch (System.InvalidOperationException err)
            {
                throw new System.InvalidOperationException(colInfos, err);
            }*/

        }
    }
}
