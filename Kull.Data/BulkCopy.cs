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
        public static void BulkInsert(this DbConnection connection, DBObjectName destinationTable, DbDataReader source)
        {
            Type bulkCopyType;
            Type optionsType;
            if(connection.GetType().FullName == "System.Data.SqlClient.SqlConnection")
            {
                bulkCopyType = Type.GetType("System.Data.SqlClient.SqlBulkCopy, System.Data.SqlClient", true);
                optionsType = Type.GetType("System.Data.SqlClient.SqlBulkCopyOptions, System.Data.SqlClient", true);
            }
            else if (connection.GetType().FullName == "Microsoft.Data.SqlClient.SqlConnection")
            {
                bulkCopyType = Type.GetType("Microsoft.Data.SqlClient.SqlBulkCopy, Microsoft.Data.SqlClient", true);
                optionsType = Type.GetType("Microsoft.Data.SqlClient.SqlBulkCopyOptions, Microsoft.Data.SqlClient", true);
            }
            else
            {
                throw new InvalidOperationException("Currently only supports Microsoft.Data.SqlClient and System.Data.SqlClient");
            }
            var tableLockValue = Convert.ChangeType(0x4, optionsType);
            var cp = Activator.CreateInstance(bulkCopyType, connection, tableLockValue, null);
            cp.GetType().GetProperty("DestinationTableName",  System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.SetProperty).SetValue(cp, destinationTable.ToString(false, true));
            var colMappings= cp.GetType().GetProperty("ColumnMappings", System.Reflection.BindingFlags.Public |
                   System.Reflection.BindingFlags.Instance |
                   System.Reflection.BindingFlags.SetProperty);
            var addMethod = colMappings.GetType().GetMethod("Add", new Type[] { typeof(string), typeof(string) });

            string colInfos = "";
            for (int i = 0; i < source.FieldCount; i++)
            {
                string columnName = source.GetName(i);
                colInfos += columnName + ": " + source.GetDataTypeName(i) + "  " + source.GetFieldType(i) + Environment.NewLine;
                addMethod.Invoke(colMappings, new object[] { source.GetName(i), columnName });
                
            }
            cp.GetType().GetMethod("WriteToServer", new Type[] {typeof(DbDataReader)}).Invoke(cp, new object[] { source });
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
