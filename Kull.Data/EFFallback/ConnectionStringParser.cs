using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kull.Data.EFFallback
{
    /// <summary>
    /// A class for getting info about a connection string
    /// </summary>
    public static class ConnectionStringParser
    {
        private enum ParseState
        {
            Start = 0,
            Key = 1,
            Equals = 2,
            Value = 3,
            InStr = 4
        }


        /// <summary>
        /// Parses a connection string and returns the Provider Connection String and the Provider.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>An object containing structured info about the connection string</returns>
        [System.Diagnostics.Contracts.Pure]
        public static ParseEFResult ParseEF(string connectionString)
        {
            var parts = Parse(connectionString);
            var provider = parts.ContainsKey("provider") ? parts["provider"] : null;
            var providerConnStr = parts.ContainsKey("provider connection string") ? parts["provider connection string"] : null;
            var providerConnStrData = providerConnStr == null ? parts : Parse(providerConnStr);
            return new ParseEFResult(provider, providerConnStr ?? connectionString, providerConnStrData);
        }

        /// <summary>
        /// Creates a connection string compatible with the entity framework
        /// </summary>
        /// <param name="providerName">The name of the provider, eg nameof (System.Data.SqlClient)</param>
        /// <param name="connectionString">The provider-specific connection string</param>
        /// <returns></returns>
        public static string CreateEF(string providerName, string connectionString)
        {
            var parts = Parse(connectionString);
            if (parts.TryGetValue("provider", out string prov) && !string.IsNullOrEmpty(prov))
            {
                return connectionString; // It is already an ef connction string
            }
            else
            {
                return "provider=" + providerName + ";provider connection string=\""
                    + connectionString.Replace("\"", "&quot;") + "\"";
            }
        }
        private static Dictionary<string, string> Parse(string connectionString)
        {
            var state = ParseState.Start;
            var values = new List<string>();
            var currentValue = "";

            for (var i = 0; i < connectionString.Length; i++)
            {
                var chr = connectionString[i];
                if (state == ParseState.Start)
                {
                    state = ParseState.Key;
                }

                if (state != ParseState.InStr)
                {
                    if (chr == '=')
                    {
                        state = ParseState.Equals;
                        values.Add(currentValue);
                        currentValue = "";
                        continue;
                    }
                    else if (chr == ';')
                    {
                        state = ParseState.Key;
                        values.Add(currentValue);
                        currentValue = "";
                        continue;
                    }
                }
                if (state == ParseState.Equals && chr == '"')
                {
                    state = ParseState.InStr;
                    currentValue = "";

                }
                else if (state == ParseState.InStr && chr == '"')
                {
                    state = ParseState.Key;
                    i++;//Jump over ;
                    values.Add(currentValue.Replace("&quot;", "\""));
                    currentValue = "";
                }
                else
                {
                    if (state == ParseState.Equals)
                    {
                        state = ParseState.Value;
                    }
                    currentValue += chr;
                }

            }
            if (currentValue.Length > 0)
                values.Add(currentValue);
            var obj = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            for (var vp = 0; vp < values.Count; vp += 2)
            {
                obj[values[vp].Trim()] = values[vp + 1];
            }
            return obj;
        }

    }
}
