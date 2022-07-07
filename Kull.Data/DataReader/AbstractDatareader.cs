using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Kull.Data.DataReader
{
    /// <summary>
    /// A helper class allow to override just GetValue in order to do something
    /// </summary>
#pragma warning disable CA1010 // Collections should implement generic interface
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public abstract class AbstractDatareader: System.Data.Common.DbDataReader
#pragma warning restore CA1710 // Identifiers should have correct suffix
#pragma warning restore CA1010 // Collections should implement generic interface
    {

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override object? this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        public override object? this[string name]
        {
            get
            {
                return GetValue(GetOrdinal(name));
            }
        }




        public override bool GetBoolean(int i)
        {
            return (bool)GetValue(i);
        }

        public override byte GetByte(int i)
        {
            return (byte)GetValue(i);
        }

        public override long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
        {
            var vl = GetValue(i);
            byte[] bvl;
            if (vl == null)
            {
                bvl = new byte[0];
            }
            else if (vl == DBNull.Value)
            {
                bvl = new byte[0];
            }
            else if (vl.GetType() == typeof(string))
            {
                bvl = System.Text.Encoding.Default.GetBytes((string)vl);
            }
            else if (vl.GetType() == typeof(byte[]))
            {
                bvl = ((byte[])vl);
            }
            else
            {
                throw new ArgumentException("Cannot convert to byte");
            }
            if (buffer == null) return bvl.Length;
            int c = 0;
            for (long of = fieldOffset; of < bvl.Length && c <= length; of++)
            {
                buffer[bufferoffset + c] = bvl[of];
                c++;
            }
            return c;
        }

        public override char GetChar(int i)
        {
            return (char)GetValue(i);
        }

        public override long GetChars(int i, long fieldOffset, char[]? buffer, int bufferoffset, int length)
        {
            var strVl = GetString(i);
            if (buffer == null) return strVl.Length;
            int c = 0;
            for (int of = (int)fieldOffset; of < strVl.Length && c < length; of++)
            {
                buffer[bufferoffset + c] = strVl[of];
                c++;
            }
            return c;
        }


        public override string GetDataTypeName(int i)
        {
            var vl = GetFieldType(i);
            if (vl == null)
                return "void"; // Good?
            if (vl == typeof(string))
                return "varchar";
            if (vl == typeof(int) || vl == typeof(int?))
                return "int";
            if (vl == typeof(long) || vl == typeof(long?))
                return "bigint";
            if (vl == typeof(byte) || vl == typeof(byte?))
                return "tinyint";
            if (vl == typeof(Guid) || vl == typeof(Guid?))
                return "uniqueidentifier";
            if (vl == typeof(byte[]))
                return "varbinary";
            if (vl == typeof(float) || vl == typeof(float?))
                return "float";
            if (vl == typeof(double) || vl == typeof(double?))
                return "float";
            if (vl == typeof(bool) || vl == typeof(bool?))
                return "bit";
            if (vl == typeof(DateTime) || vl == typeof(DateTime?))
                return "datetime";
            if (vl == typeof(DateTimeOffset) || vl == typeof(DateTimeOffset?))
                return "datetimeoffset";
            return vl.GetType().Name; // Not great
        }

        public override DateTime GetDateTime(int i)
        {
            return (DateTime)GetValue(i);
        }

        public override decimal GetDecimal(int i)
        {
            return (decimal)GetValue(i);
        }

        public override double GetDouble(int i)
        {
            return (double)GetValue(i);
        }

        public override Type GetFieldType(int i)
        {
            var vl = GetValue(i);
            if (vl == null)
                return typeof(DBNull);
            return vl.GetType();
        }

        public override float GetFloat(int i)
        {
            return (float)GetValue(i);
        }

        public override Guid GetGuid(int i)
        {
            return (Guid)GetValue(i);
        }

        public override short GetInt16(int i)
        {
            return (short)GetValue(i);
        }

        public override int GetInt32(int i)
        {
            return (int)GetValue(i);
        }

        public override long GetInt64(int i)
        {
            return (long)GetValue(i);
        }

        protected virtual SchemaDataTableInfo GetSchemaDataTableInfo(int i)
        {

            Type t = GetFieldType(i);
            string name = GetName(i);
            string dataTypeName = GetDataTypeName(i);
            return new SchemaDataTableInfo(ColumnName: name, ColumnOrdinal: i, ColumnSize: -1,
                NumericPrecision: null, NumericScale: null, DataType: dataTypeName, ProviderType: t, IsLong: true);

        }

        protected System.Collections.Generic.IEnumerable<SchemaDataTableInfo> GetSchemaDataTableInfo()
        {
            for(int i=0; i<this.FieldCount; i++)
            {
                yield return GetSchemaDataTableInfo(i);
            }
        }

        public override DataTable GetSchemaTable()
        {
            //  see https://docs.microsoft.com/en-us/dotnet/api/system.data.datatablereader.getschematable?view=net-6.0#remarks
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ColumnName", typeof(string)));
            dt.Columns.Add(new DataColumn("ColumnOrdinal", typeof(int)));
            dt.Columns.Add(new DataColumn("ColumnSize", typeof(int)));
            dt.Columns.Add(new DataColumn("NumericPrecision", typeof(int?)));
            dt.Columns.Add(new DataColumn("NumericScale", typeof(int?)));
            dt.Columns.Add(new DataColumn("DataType", typeof(string)));
            dt.Columns.Add(new DataColumn("ProviderType", typeof(Type)));
            dt.Columns.Add(new DataColumn("IsLong", typeof(bool)));
            dt.Columns.Add(new DataColumn("AllowDBNull", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsReadOnly", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsRowVersion", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsUnique", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsKey", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsAutoIncrement", typeof(bool)));
            dt.Columns.Add(new DataColumn("BaseCatalogName", typeof(string)));
            dt.Columns.Add(new DataColumn("BaseSchemaName", typeof(string)));
            dt.Columns.Add(new DataColumn("BaseTableName", typeof(string)));
            dt.Columns.Add(new DataColumn("BaseColumnName", typeof(string)));
            dt.Columns.Add(new DataColumn("AutoIncrementSeed", typeof(int)));
            dt.Columns.Add(new DataColumn("AutoIncrementStep", typeof(int)));
            dt.Columns.Add(new DataColumn("DefaultValue", typeof(string)));
            dt.Columns.Add(new DataColumn("Expression", typeof(string)));
            dt.Columns.Add(new DataColumn("ColumnMapping", typeof(MappingType)));
            dt.Columns.Add(new DataColumn("BaseTableNamespace", typeof(string)));
            dt.Columns.Add(new DataColumn("BaseColumnNamespace", typeof(string)));
            var infos = GetSchemaDataTableInfo();
            foreach (var info in infos)
            {
                var row = dt.NewRow();
                row["ColumnName"]=info.ColumnName;                
                row["ColumnOrdinal"]=info.ColumnOrdinal;
                row["ColumnSize"]=info.ColumnSize;        
                row["NumericPrecision"]=info.NumericPrecision;  
                row["NumericScale"]=info.NumericScale;         
                row["DataType"]=info.DataType;          
                row["ProviderType"]=info.ProviderType;        
                row["IsLong"]=info.IsLong;           
                row["AllowDBNull"]=info.AllowDBNull;  
                row["IsReadOnly"]=info.IsReadOnly;      
                row["IsRowVersion"]=info.IsRowVersion;      
                row["IsUnique"]=info.IsUnique;              
                row["IsKey"]=info.IsKey;             
                row["IsAutoIncrement"]=info.IsAutoIncrement;  
                row["BaseCatalogName"]=info.BaseCatalogName; 
                row["BaseSchemaName"]=info.BaseSchemaName; 
                row["BaseTableName"]=info.BaseTableName;     
                row["BaseColumnName"]=info.BaseColumnName;     
                row["AutoIncrementSeed"]=info.AutoIncrementSeed;
                row["AutoIncrementStep"]=info.AutoIncrementStep;
                row["DefaultValue"]=info.DefaultValue;           
                row["Expression"]=info.Expression;              
                row["ColumnMapping"]=info.ColumnMapping;    
                row["BaseTableNamespace"]=info.BaseTableNamespace;
                row["BaseColumnNamespace"]=info.BaseColumnNamespace;

                dt.Rows.Add(row);
            }
            return dt;
        }

        public override string GetString(int i)
        {
            return (string)GetValue(i);
        }


        public override int GetValues(object[] values)
        {
            int vls = 0;
            for (int i = 0; i < Math.Min(values.Length, FieldCount); i++)
            {
                values[i] = GetValue(i);
                vls = i;
            }
            return vls;
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Read());
        }

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.NextResult());
        }
    }
}