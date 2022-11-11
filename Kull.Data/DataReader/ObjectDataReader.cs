using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kull.Data.DataReader
{
    /// <summary>
    /// A class for converting a list of dictionaries to a DataReader.
    /// </summary>
#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable CA1010 // Collections should implement generic interface
    public class ObjectDataReader : AbstractDatareader
#pragma warning restore CA1010 // Collections should implement generic interface
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        private readonly IEnumerator<IReadOnlyDictionary<string, object?>> baseValues;
        private readonly string[] names;
        private readonly Type[]? types;
        private bool? firstRead = null;

        /// <summary>
        /// Missing fields in dictionary are allowed.
        /// </summary>
        public bool AllowMissing { get; } = false;

        public override bool HasRows => (firstRead == null || firstRead == true);

        /// <summary>
        /// This creates a new ObjectDataReader
        /// </summary>
        /// <param name="baseValues">The values to base on</param>
        /// <param name="names">The names are available only after the first read which does sometimes make trouble.
        /// If you know the names before, set them here (alternatively pass a list)</param>
        /// <param name="types">The types of the fields</param>
        /// <param name="allowMissingFields">Allow missing fields in dictionary.</param>
        public ObjectDataReader(IEnumerable<IReadOnlyDictionary<string, object?>> baseValues, string[] names,
                Type[] types,
                bool allowMissingFields)
        {
            this.baseValues = baseValues.GetEnumerator();
            firstRead = this.baseValues.MoveNext();// Do first read to have HasRows

            this.names = names;
            this.types = types;
            this.AllowMissing = allowMissingFields;
        }


        /// <summary>
        /// This creates a new ObjectDataReader
        /// </summary>
        /// <param name="baseValues">The values to base on</param>
        /// <param name="names">The names are available only after the first read which does sometimes make trouble.
        /// If you know the names before, set them here (alternatively pass a list)</param>
        /// <param name="deepTypeScan">Set this if you want to scan the whole list or as much as neccessary to get field types</param>
        public ObjectDataReader(IEnumerable<IReadOnlyDictionary<string, object?>> baseValues, string[]? names = null,
                bool deepTypeScan = false)
        {
            this.baseValues = baseValues.GetEnumerator();
            firstRead = this.baseValues.MoveNext();

            this.names = names ?? this.baseValues.Current.Keys.ToArray();
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
                            var vl = item[key];
                            if (vl != null && vl != DBNull.Value)
                            {
                                types[i] = vl.GetType();
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
                if (AllowMissing && !this.baseValues.Current.ContainsKey(name)) return null;
                return this.baseValues.Current[name];
            }
        }


        public override int FieldCount => names.Length;

        public override void Close()
        {
            if (!_isClosed)
            {
                baseValues.Dispose();
            }
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isClosed)
            {
                baseValues.Dispose();
                _isClosed = true;
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

        public override object? GetValue(int i)
        {
            if (AllowMissing && !this.baseValues.Current.ContainsKey(names[i])) return null;
            return this.baseValues.Current[names[i]];
        }


        public override bool IsDBNull(int i)
        {
            if (AllowMissing && !this.baseValues.Current.ContainsKey(names[i])) return true;
            return baseValues.Current[names[i]] == DBNull.Value || baseValues.Current[names[i]] == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            if (firstRead != null)
            {
                bool vl = firstRead.Value;
                firstRead = null;
                return vl; // Do not use MoveNext
            }
            return this.baseValues.MoveNext();
        }



    }
}
