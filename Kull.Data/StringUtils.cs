using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Kull.Data
{
    /// <summary>
    /// A Helper class containing string utilities
    /// </summary>
    public static class StringUtils
    {

        /// <summary>
        /// Replaces a string 
        /// </summary>
        /// <param name="input">The string to search in</param>
        /// <param name="search">The string to search for</param>
        /// <param name="replace">The string which will be inserted. If it contains @@Found@@, this will be replaced with the found string. See example</param>
        /// <param name="cultureInfo">The culture to be used</param>
        /// <example>"hans fritz peter".ReplaceInsensitive("Fritz", "&lt;em&gt;@@Found@@&lt;/em&gt;") == "hans &lt;em&gt;frist&lt;/em&gt; peter"</example>
        /// <returns></returns>
        [System.Diagnostics.Contracts.Pure]
        public static string? ReplaceInsensitive(this string? input, string search, string replace,
                CultureInfo cultureInfo)
        {

            if (input == null)
                return null;
            if (search == null)
                return input;
            if (replace == null)
                replace = "";
            var linput = input.ToLower(cultureInfo);
            var lsearch = search.ToLower(cultureInfo);
            int searchStart = 0;
            int searchIndex = linput.IndexOf(lsearch, searchStart, StringComparison.OrdinalIgnoreCase);
            while (searchIndex != -1)
            {
                string before = input.Substring(0, searchIndex);
                string toReplace = input.Substring(searchIndex, lsearch.Length);
                string after = input.Substring(searchIndex + lsearch.Length);
                string replaceStr = replace.Replace("@@Found@@", toReplace).Replace("{{Found}}", toReplace);
                input = before + replaceStr + after;
                linput = input.ToLower(cultureInfo);
                searchStart = (before + replaceStr).Length;
                if (searchStart >= linput.Length)
                    break;
                searchIndex = linput.IndexOf(lsearch, searchStart, StringComparison.OrdinalIgnoreCase);
            }
            return input;


        }

        /// <summary>
        /// Replaces a string 
        /// </summary>
        /// <param name="input">The string to search in</param>
        /// <param name="search">The string to search for</param>
        /// <param name="replace">The string which will be inserted. If it contains @@Found@@, this will be replaced with the found string. See example</param>
        /// <example>"hans fritz peter".ReplaceInsensitive("Fritz", "&lt;em&gt;@@Found@@&lt;/em&gt;") == "hans &lt;em&gt;frist&lt;/em&gt; peter"</example>
        /// <returns></returns>
        [System.Diagnostics.Contracts.Pure]
        public static string? ReplaceInsensitive(this string? input, string search, string replace)
        {
            return ReplaceInsensitive(input, search, replace, CultureInfo.CurrentCulture);
        }
    }
}
