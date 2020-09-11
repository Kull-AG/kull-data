using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kull.Data.DataReader
{
    /// <summary>
    /// A class for Adding columns to a DataReader.
    /// </summary>
    public class WrappedDataReader : AbstractDatareader
    {
        private readonly IDataReader baseReader;
        private readonly DbDataReader? baseReaderDb;
        private readonly IDictionary<string, object> additionalValues;
        private readonly string[] additionalColumns;
        private readonly int baseFieldCount;
        private readonly string[] names;

        private bool? firstRead = null;

        public override bool HasRows => (firstRead == null || firstRead == true);


        /// <summary>
        /// This creates a new WrappedDataReader
        /// </summary>
        /// <param name="baseReader">The basedatareader</param>
        /// <param name="additionalValues">The columns to add to the source</param>
        /// <param name="columnNameMapping">The Mapping for the column names</param>
        public WrappedDataReader(IDataReader baseReader, IDictionary<string, object> additionalValues,
                 IDictionary<string, string>? columnNameMapping = null)
        {
            if (baseReader.FieldCount < 0)
            {
                firstRead = baseReader.Read();
                if (baseReader.FieldCount < 0)
                {
                    throw new InvalidOperationException("Field count must be known!");
                }
            }
            this.baseReader = baseReader;
            this.baseReaderDb = baseReader as DbDataReader;
            this.additionalValues = additionalValues;
            this.additionalColumns = new string[additionalValues.Count];
            this.baseFieldCount = baseReader.FieldCount;
            int i = 0;

            foreach (var item in additionalValues)
            {
                additionalColumns[i++] = item.Key;

            }
            names = new string[baseReader.FieldCount + additionalColumns.Length];

            bool supportsNames = true;
            if (names.Length > 0)
            { // Not nice, but the only method
                try { baseReader.GetName(0); }
                catch (NotSupportedException)
                {
                    supportsNames = false;
                }
            }

            for (int j = 0; j < names.Length; j++)
            {
                string bName;
                if (j < baseFieldCount)
                {
                    bName = supportsNames ? baseReader.GetName(j) : "F" + (j + 1).ToString();
                }
                else
                {
                    bName = additionalColumns[j - baseFieldCount];
                }
                if (columnNameMapping != null)
                {
                    if (columnNameMapping.ContainsKey(bName))
                    {
                        bName = columnNameMapping[bName];
                        break;
                    }
                }
                if (!names.Contains(bName, StringComparer.CurrentCultureIgnoreCase))
                {
                    names[j] = bName;
                }
                else
                {
                    for (int k = 1; ; k++)
                    {
                        if (!names.Contains(bName + "_" + k.ToString(), StringComparer.CurrentCultureIgnoreCase))
                        {
                            names[j] = bName + "_" + k.ToString();
                            break;
                        }
                    }
                }
            }
        }


        private bool IsBaseColumn(int i)
        {
            return i < baseFieldCount;
        }

        /// <summary>
        /// Gets a value of the given column index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override object? this[int i]
        {
            get
            {
                return IsBaseColumn(i) ? baseReader[i] : additionalValues[additionalColumns[i - baseFieldCount]];
            }
        }

        /// <summary>
        /// Gets a value of the given Column Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object? this[string name]
        {
            get
            {
                int i = GetOrdinal(name);
                return this[i];
            }
        }

        public override int Depth => baseReader.Depth;

        public override bool IsClosed => baseReader.IsClosed;

        public override int RecordsAffected => baseReader.RecordsAffected;

        public override int FieldCount => baseFieldCount + additionalColumns.Length;

        public override void Close()
        {
            baseReader.Close();
        }

        protected override void Dispose(bool disposing)
        {
            baseReader.Dispose();
        }

        public override bool GetBoolean(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetBoolean(i) : (bool)GetValue(i);
        }

        public override byte GetByte(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetByte(i) : (byte)GetValue(i);
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (IsBaseColumn(i))
            {
                return baseReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
            }
            throw new NotImplementedException("The GetBytes method has to be overwritten");
        }

        public override char GetChar(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetChar(i) : (char)GetValue(i);
        }

        public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            if (IsBaseColumn(i))
                return baseReader.GetChars(i, fieldOffset, buffer, bufferoffset, length);
            return base.GetChars(i, fieldOffset, buffer, bufferoffset, length);
        }

        public override string GetDataTypeName(int i)
        {
            if (IsBaseColumn(i))
                return baseReader.GetDataTypeName(i);
            return base.GetDataTypeName(i);
        }

        public override DateTime GetDateTime(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetDateTime(i) : (DateTime)GetValue(i);
        }

        public override decimal GetDecimal(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetDecimal(i) : (decimal)GetValue(i);
        }

        public override double GetDouble(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetDouble(i) : (double)GetValue(i);
        }

        public override Type GetFieldType(int i)
        {
            if (IsBaseColumn(i))
                return baseReader.GetFieldType(i);
            return base.GetFieldType(i);
        }

        public override float GetFloat(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetFloat(i) : (float)GetValue(i);
        }

        public override Guid GetGuid(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetGuid(i) : (Guid)GetValue(i);
        }

        public override short GetInt16(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetInt16(i) : (short)GetValue(i);
        }

        public override int GetInt32(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetInt32(i) : (int)GetValue(i);
        }

        public override long GetInt64(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetInt64(i) : (long)GetValue(i);
        }

        public override string GetName(int i)
        {
            return names[i];
        }

        public override int GetOrdinal(string name)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return i;
            }
            return -1;
        }

        public override string GetString(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetString(i) : (string)GetValue(i);
        }

        public override object GetValue(int i)
        {
            return IsBaseColumn(i) ? baseReader.GetValue(i) : additionalValues[additionalColumns[i - baseFieldCount]];
        }
#if !NET2
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            return IsBaseColumn(ordinal) ? (baseReaderDb == null ? Task.FromResult((T)baseReader.GetValue(ordinal))
                    : baseReaderDb.GetFieldValueAsync<T>(ordinal, cancellationToken))
                : Task.FromResult((T)additionalValues[additionalColumns[ordinal - baseFieldCount]]);
        }

        public override T GetFieldValue<T>(int ordinal)
        {
            if (!IsBaseColumn(ordinal))
            {
                return (T)additionalValues[additionalColumns[ordinal - baseFieldCount]];
            }
            if (baseReaderDb != null)
                return baseReaderDb.GetFieldValue<T>(ordinal);
            return (T)baseReader.GetValue(ordinal);
        }
#endif
        public override bool IsDBNull(int i)
        {
            return IsBaseColumn(i) ? baseReader.IsDBNull(i) : additionalValues[additionalColumns[i - baseFieldCount]] == DBNull.Value;
        }
        
        public override bool NextResult()
        {
            return baseReader.NextResult();
        }

        public override bool Read()
        {
            if (firstRead != null)
            {
                // Skip first read
                var oFirstRead = firstRead.Value;
                firstRead = null;
                return oFirstRead;
            }
            return baseReader.Read();
        }

#if !NET2
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return baseReaderDb?.ReadAsync(cancellationToken) ?? Task.FromResult(Read());
        }

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return baseReaderDb?.NextResultAsync(cancellationToken) ?? Task.FromResult(NextResult());
        }
#endif
#if ASYNCSTREAM
        public override Task CloseAsync()
        {
            if(baseReaderDb != null)
            {
                return baseReaderDb.CloseAsync();
            }
            baseReader.Close();
            return Task.CompletedTask;
        }
#endif
    }
}
