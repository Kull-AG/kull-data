#if ASYNCSTREAM
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
#pragma warning disable CA1010 // Collections should implement generic interface
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public class AsyncObjectDataReader : AbstractDatareader
#pragma warning restore CA1710 // Identifiers should have correct suffix
#pragma warning restore CA1010 // Collections should implement generic interface
    {
        private readonly IAsyncEnumerator<IReadOnlyDictionary<string, object?>> baseValues;
        private string[]? names;
        private bool isClosed = false;
        private Type[]? types;
        private bool? firstRead = null;

        public override bool HasRows => (firstRead == null || firstRead == true);

        Task initTask;

        /// <summary>
        /// This creates a new ObjectDataReader
        /// </summary>
        /// <param name="baseValues">The values to base on</param>
        /// <param name="names">The names are available only after the first read which does sometimes make trouble.
        /// If you know the names before, set them here (alternatively pass a list)</param>
        public AsyncObjectDataReader(IAsyncEnumerable<IReadOnlyDictionary<string, object?>> baseValues, string[]? names = null)
        {
            this.baseValues = baseValues.GetAsyncEnumerator();
            if (names == null)
            {
                this.initTask = Init();
            }
            else
            {
                initTask = Task.CompletedTask;
            }
        }

        private async Task Init()
        {
            firstRead = await this.baseValues.MoveNextAsync();

            this.names = this.baseValues.Current.Keys.ToArray();
            types = new Type[this.names.Length];
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

        public override int FieldCount
        {
            get
            {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                if (names == null) throw new InvalidOperationException("Either do a read first or set names on conststructor");
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
                return names.Length;
            }
        }

        public override Task CloseAsync()
        {
            if (!isClosed)
            {
                var t = baseValues.DisposeAsync();
                isClosed = true;
                return t.AsTask();
            }
            return Task.CompletedTask;
        }

        public override ValueTask DisposeAsync()
        {
            if (!isClosed)
            {
                var t = baseValues.DisposeAsync();
                isClosed = true;
                return t;
            }
            return default;
        }

        public override string GetName(int i)
        {
            if (names == null) throw new InvalidOperationException("Either do a read first or set names on conststructor");
            return names[i];
        }

        public override int GetOrdinal(string name)
        {
            if (names == null) throw new InvalidOperationException("Either do a read first or set names on conststructor");
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return i;
            }
            return -1;
        }

        public override object? GetValue(int i)
        {
            if (names == null) throw new InvalidOperationException("Either do a read first or set names on conststructor");
            return this.baseValues.Current[names[i]];
        }


        public override bool IsDBNull(int i)
        {
            if (names == null) throw new InvalidOperationException("Either do a read first or set names on conststructor");
            return baseValues.Current[names[i]] == DBNull.Value || baseValues.Current[names[i]] == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }


        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            await initTask.ConfigureAwait(false);
            if (firstRead != null)
            {
                bool vl = firstRead.Value;
                firstRead = null;
                return vl; // Do not use MoveNext
            }
            return await this.baseValues.MoveNextAsync();
        }



    }
}
#endif