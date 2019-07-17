using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kull.Data.DataReader
{
    /// <summary>
    /// A class for converting a list of dictionaries to a DataReader.
    /// </summary>
    public class ObjectDataReader : AbstractDatareader
    {
        private readonly IEnumerator<IDictionary<string, object>> baseValues;
        private string[] names;
        private bool isClosed = false;
        private Type[]? types;
        private bool firstRowEnumerated = false;

        /// <summary>
        /// This creates a new ObjectDataReader
        /// </summary>
        /// <param name="baseValues">The values to base on</param>
        /// <param name="names">The names are available only after the first read which does sometimes make trouble.
        /// If you know the names before, set them here (alternatively pass a list)</param>
        /// <param name="deepTypeScan">Set this if you want to scan the whole list or as much as neccessary to get field types</param>
        public ObjectDataReader(IEnumerable<IDictionary<string, object>> baseValues, string[]? names = null,
                bool deepTypeScan = false)
        {
            this.baseValues = baseValues.GetEnumerator();
            firstRowEnumerated = true;
            this.baseValues.MoveNext();

            this.names = names ?? this.baseValues.Current.Keys.ToArray(); ;
            if (deepTypeScan)
            {
                // We have to enumerate it twice in this case (even though we might do not have to scan the full list)
                foreach (var item in baseValues)
                {
                    if (names == null)
                    {
                        names = item.Keys.ToArray();
                    }
                    if (types == null)
                    {
                        types = new Type[names.Length];
                    }
                    bool oneNull = false;
                    for (int i = 0; i < names.Length; i++)
                    {
                        string key = names[i];
                        if (types[i] == null)
                        {
                            if (item[key] != null && item[key] != DBNull.Value)
                            {
                                types[i] = item[key].GetType();
                            }
                            else
                            {
                                oneNull = true;
                            }
                        }
                    }
                    if (!oneNull)
                        break;
                }
            }

        }

        /// <summary>
        /// Gets the type of a field
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override Type GetFieldType(int i)
        {
            if (types != null)
            {
                if (types[i] != null)
                {
                    return types[i];
                }
                else
                {
                    return (types[i] = base.GetFieldType(i));
                }
            }
            return base.GetFieldType(i);
        }




        public override object? this[string name]
        {
            get
            {
                return this.baseValues.Current[name];
            }
        }

        public override int Depth => 0;

        public override bool IsClosed => isClosed;

        public override int RecordsAffected => 0;

        public override int FieldCount => names.Length;

        public override void Close()
        {
            if (!isClosed)
            {
                baseValues.Dispose();
                isClosed = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!isClosed)
            {
                baseValues.Dispose();
                isClosed = true;
            }
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

        public override object GetValue(int i)
        {
            return this.baseValues.Current[names[i]];
        }


        public override bool IsDBNull(int i)
        {
            return baseValues.Current[names[i]] == DBNull.Value || baseValues.Current[names[i]] == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            if (firstRowEnumerated)
            {
                firstRowEnumerated = false;
                return true; // Do not use MoveNext
            }
            return this.baseValues.MoveNext();
        }
    }
}
