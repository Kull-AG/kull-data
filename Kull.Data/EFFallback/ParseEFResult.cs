using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace Kull.Data.EFFallback
{
    /// <summary>
    /// Represents a Result of the parsing of a connection string
    /// </summary>
    public struct ParseEFResult : IEquatable<ParseEFResult>
    {
        /// <summary>
        /// The provider name. Will be null if not an entity framework connection string
        /// </summary>
        public readonly string? Provider;

        /// <summary>
        /// The provider connection string
        /// </summary>
        public readonly string ConnectionString;

        /// <summary>
        /// Contains the data of the connection string. This will be Data Source as Key and so on.
        /// </summary>
        public readonly IReadOnlyDictionary<string, string> ConnectionStringData;

        /// <summary>
        /// Creates new Results
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connStr"></param>
        /// <param name="data"></param>
        internal ParseEFResult(string? provider, string connStr, Dictionary<string, string> data)
        {
            this.Provider = provider;
            this.ConnectionString = connStr;
            this.ConnectionStringData = data;
        }

        /// <summary>
        /// Returns true if Both, the provider and the ConnectionString Equal
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is ParseEFResult res)
            {
                return Equals(res);
            }
            return false;
        }

        /// <summary>
        /// Returns true if Both, the provider and the ConnectionString Equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ParseEFResult other)
        {
            return other.ConnectionString == this.ConnectionString &&
                      other.Provider == this.Provider;
        }

        /// <summary>
        /// Computes a hashcode out of the connection string and the provider
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ConnectionString.GetHashCode() ^ (Provider?.GetHashCode()??0);
        }

        /// <summary>
        /// Returns true if both the provider and the connection string equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ParseEFResult left, ParseEFResult right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns  false  if both the provider and the connection string equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ParseEFResult left, ParseEFResult right)
        {
            return !(left == right);
        }
    }
}


#nullable restore