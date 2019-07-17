using System;
using System.Data;

namespace Kull.Data.DataReader
{
    /// <summary>
    /// A helper class allow to override just GetValue in order to do something
    /// </summary>
    public abstract class AbstractDatareader: IDataReader
    {

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual object? this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        public virtual object? this[string name]
        {
            get
            {
                return GetValue(GetOrdinal(name));
            }
        }

        public abstract int Depth { get; }

        public abstract bool IsClosed { get; }

        public abstract int RecordsAffected { get; }

        public abstract int FieldCount { get; }

        public abstract void Close();


        public virtual bool GetBoolean(int i)
        {
            return (bool)GetValue(i);
        }

        public virtual byte GetByte(int i)
        {
            return (byte)GetValue(i);
        }

        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
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

        public virtual char GetChar(int i)
        {
            return (char)GetValue(i);
        }

        public virtual long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
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

        public virtual IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public virtual string GetDataTypeName(int i)
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

        public virtual DateTime GetDateTime(int i)
        {
            return (DateTime)GetValue(i);
        }

        public virtual decimal GetDecimal(int i)
        {
            return (decimal)GetValue(i);
        }

        public virtual double GetDouble(int i)
        {
            return (double)GetValue(i);
        }

        public virtual Type GetFieldType(int i)
        {
            var vl = GetValue(i);
            if (vl == null)
                return typeof(DBNull);
            return vl.GetType();
        }

        public virtual float GetFloat(int i)
        {
            return (float)GetValue(i);
        }

        public virtual Guid GetGuid(int i)
        {
            return (Guid)GetValue(i);
        }

        public virtual short GetInt16(int i)
        {
            return (short)GetValue(i);
        }

        public virtual int GetInt32(int i)
        {
            return (int)GetValue(i);
        }

        public virtual long GetInt64(int i)
        {
            return (long)GetValue(i);
        }

        public abstract string GetName(int i);

        public abstract int GetOrdinal(string name);

        public virtual DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public virtual string GetString(int i)
        {
            return (string)GetValue(i);
        }

        public abstract object GetValue(int i);

        public virtual int GetValues(object[] values)
        {
            int vls = 0;
            for (int i = 0; i < Math.Min(values.Length, FieldCount); i++)
            {
                values[i] = GetValue(i);
                vls = i;
            }
            return vls;
        }

        public abstract bool IsDBNull(int i);

        public abstract bool NextResult();

        public abstract bool Read();

        #region IDisposable Support
        protected abstract void Dispose(bool disposing);

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AbstractDatareader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}