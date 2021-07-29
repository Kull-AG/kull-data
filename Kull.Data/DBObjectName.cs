using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kull.Data
{
    /// <summary>
    /// A helper class for creating a tablename. This is used to store the DefaultSchema as well (which should not be hard-coded)
    /// </summary>
    [System.Diagnostics.Contracts.Pure]
    public class DBObjectName : IComparable
    {
        private static string defaultSchema = "dbo";

        /// <summary>
        /// Default Schema for Tool Tables. Default is "dbo"
        /// </summary>
        public static string DefaultSchema
        {
            get { return defaultSchema; }
            set { defaultSchema = value; }
        }

        private readonly string? dbName;

        /// <summary>
        /// Name of the database
        /// </summary>
        public string? DataBaseName
        {
            get { return dbName; }
        }

        private readonly string schema;
        /// <summary>
        /// Name of the Schema
        /// </summary>
        public string Schema
        {
            get
            {
                return schema ?? DBObjectName.DefaultSchema;
            }
        }

        private readonly string name;

        /// <summary>
        /// Name of the Object
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Creates a new DBObjectName
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <param name="db"></param>
        public DBObjectName(string schema, string name, string? db = null)
        {
            this.dbName = db;
            this.schema = schema;
            this.name = name;
        }

        /// <summary>
        /// returns Schema.Name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Schema + "." + Name;
        }

        /// <summary>
        /// Returns Databasename.schema.name
        /// </summary>
        /// <param name="withDatabase">Whether to include the Databasename or not</param>
        /// <returns>Databasename.schema.name or schema.name</returns>
        public string ToString(bool withDatabase)
        {
            if (withDatabase && DataBaseName != null)
                return DataBaseName + "." + ToString();
            return ToString();
        }

        /// <summary>
        /// Returns the Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString(true).ToLower().GetHashCode();
        }

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is DBObjectName obj2)
            {
                return obj2.ToString(true).ToLower() == this.ToString(true).ToLower();
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Gets the DBObject name from a string
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The Db object name</returns>
        public static DBObjectName? FromString(string? str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            var parts = new List<string>();
            List<char> curPart = new List<char>();
            bool openQuote = false;
            bool openBracket = false;
            foreach (char c in str!)
            {
                if (c == '"' && !openBracket)
                {
                    openQuote = !openQuote;
                }
                else if (c == '[' && !openQuote)
                {
                    openBracket = true;
                }
                else if (c == ']' && !openQuote)
                {
                    openBracket = false;
                }

                else if (!openQuote && !openBracket && c == '.')
                {
                    parts.Add(new string(curPart.ToArray()));
                    curPart.Clear();
                }
                else
                {
                    curPart.Add(c);

                }
            }
            if (curPart.Count > 0)
            {
                parts.Add(new string(curPart.ToArray()));
                curPart.Clear();
            }
            if (parts.Count == 0)
                throw new ArgumentException("Invalid db object name");
            if (parts.Count == 1)
            {
                return new DBObjectName(DBObjectName.DefaultSchema, parts[0]);
            }
            else if (parts.Count == 2)
            {
                return new DBObjectName(parts[0], parts[1]);
            }
            else if (parts.Count == 3)
            {
                return new DBObjectName(parts[1], parts[2], parts[0]);
            }
            else
            {
                throw new NotSupportedException("Only database level names supported");
            }
        }

        /// <summary>
        /// Just a little helper for people who don't like writing lots of code
        /// </summary>
        public static implicit operator DBObjectName(string str)
        {
            return FromString(str)!;
        }

        /// <summary>
        /// Tests 2 objects for equality
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator ==(DBObjectName? obj1, DBObjectName? obj2)
        {
            if (object.ReferenceEquals(obj1, obj2))
            {
                return true;
            }
            if ((object?)obj1 == null || (object?)obj2 == null)
                return false;
            return obj1.Equals(obj2);
        }

        /// <summary>
        /// Tests two objects for beeing unequal
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator !=(DBObjectName? obj1, DBObjectName? obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>
        /// Compares the string representations of two objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object? obj)
        {
            if (object.ReferenceEquals(obj, this))
                return 0;
            if (obj == null)
                return -1;
            if (obj is DBObjectName name)
            {
                return this.ToString(true).ToLower().CompareTo(name.ToString(true).ToLower());
            }
            else
            {
                return this.ToString(true).ToLower().CompareTo(obj.ToString());
            }
        }

        /// <summary>
        /// String comparision. Mainly usefol for sorting in a list. Compares ToString(true)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(DBObjectName? left, DBObjectName? right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        /// <summary>
        /// String comparision. Mainly usefol for sorting in a list. Compares ToString(true)
        /// </summary>
        public static bool operator <=(DBObjectName? left, DBObjectName? right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }
        /// <summary>
        /// String comparision. Mainly usefol for sorting in a list. Compares ToString(true)
        /// </summary>

        public static bool operator >(DBObjectName? left, DBObjectName? right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        /// <summary>
        /// String comparision. Mainly usefol for sorting in a list. Compares ToString(true)
        /// </summary>
        public static bool operator >=(DBObjectName? left, DBObjectName? right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }
}
