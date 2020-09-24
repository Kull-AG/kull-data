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
    public abstract class AbstractDatareader: System.Data.Common.DbDataReader
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

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            // TODO: Testing
            var vl = GetValue(i);
            byte[] bvl;
            if (vl == null)
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

        public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            var strVl = GetString(i);

            // TODO: Testing
            int c = 0;
            for (int of = (int)fieldOffset; of < strVl.Length && c <= length; of++)
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


        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
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