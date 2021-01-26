using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kull.Data.Reporting
{
    /// <summary>
    /// Helper class for calling Reporting Services Reports
    /// </summary>
    public class ReportCall
    {
        /// <summary>
        /// The url of the server to be used by default
        /// </summary>
        public static string? DefaultReportServerUrl { get; set; }

        /// <summary>
        /// Wheter or not a server supports by default Office 2007 Formats
        /// </summary>
        public static bool DefaultSupports2007Formats { get; set; } = true;

        /// <summary>
        /// Set this boolean to false if you do not want to make exports to EXCELOPENXML (.xlsx) or to WORDOPENXML (.docx) but to EXCEL (.xls) or to WORD(.doc)
        /// </summary>
        public bool Support2007Formats { get; set; }

        /// <summary>
        /// Creates a new instance for Calling a Report
        /// </summary>
        public ReportCall()
        {
            this.ReportFormat = ReportFormat.Html;
            this.Support2007Formats = DefaultSupports2007Formats;
            this.reportServerURL = DefaultReportServerUrl;
        }

        /// <summary>
        ///  Creates a new instance for Calling a Report with a name
        /// </summary>
        /// <param name="name">The name of the report</param>
        public ReportCall(string name)
            : this()
        {
            this.ReportName = name;
        }

        private string? reportServerURL;

        /// <summary>
        /// The Url of the location of the reports
        /// </summary>
        /// <example>https://sharepoint2010.kull.ch/sites/finma/Reports/
        /// </example>
        public string? ReportServerURL
        {
            get { return reportServerURL; }
            set { reportServerURL = value; }
        }
        private string? reportName;

        /// <summary>
        /// The rdl FileName
        /// </summary>
        /// <example>ExportDashboard.rdl
        /// </example>
        public string? ReportName
        {
            get { return reportName; }
            set { reportName = value; }
        }

        private Dictionary<string, object> parameters = new Dictionary<string, object>();

        /// <summary>
        /// Returns all parameters. You usually do not want to manipulate parameters through this collection but through SetParameter
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get { return parameters; }
        }

        /// <summary>
        /// The Format of the report
        /// </summary>
        public ReportFormat ReportFormat { get; set; }

        /// <summary>
        /// Overwrites a url parameter if it already exists, or adds a new one if it does not exist yet
        /// </summary>
        /// <param name="key">The Key to set</param>
        /// <param name="value">The value to set</param>
        public ReportCall SetParameter(string key, object value)
        {
            if (Parameters.ContainsKey(key))
                Parameters[key] = value;
            else
                Parameters.Add(key, value);
            return this;
        }

        /// <summary>
        /// Overwrites multiple url parameter if it already exists, or adds a new one if it does not exist yet
        /// </summary>
        /// <param name="paramobj">Any object, usually constructed with "new", eg. SetParameters(new { Name="hans", Id=23 })</param>
        public ReportCall SetParameters<T>(T paramobj)
        {
            if (paramobj == null) return this;
            var tType = typeof(T);
            var properties = (tType == typeof(object) ? paramobj.GetType() : tType)
            .GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var prop in properties)
            {
                this.SetParameter(prop.Name, prop.GetValue(paramobj, null));
            }
            return this;
        }

#pragma warning disable CA1055 // Uri return values should not be strings
        // warning suppressed because of compat

        /// <summary>
        /// Gets the url as a string. You can use ToString as well for this
        /// </summary>
        public string GetUrl()
#pragma warning restore CA1055 // Uri return values should not be strings
        {

            string url = (ReportServerURL ?? "") + ReportName;
            AddReportParameter(ref url, "rs:Command", "Render");
            if (!Support2007Formats)
            {
                if (this.ReportFormat == ReportFormat.Excel)
                    this.ReportFormat = ReportFormat.Excel2003;
                else if (this.ReportFormat == ReportFormat.Word)
                    this.ReportFormat = ReportFormat.Word2003;

            }
            string? format = this.ReportFormat == null ? null : this.ReportFormat.RSParam;
            if (format != null)
            {
                AddReportParameter(ref url, "rs:Format", format);
            }
            else
            {
                AddReportParameter(ref url, "rc:Parameters", false);
            }
            if (Parameters != null)
            {
                foreach (var parameter in Parameters)
                {
                    AddReportParameter(ref url, parameter.Key, parameter.Value);
                }
            }
            return url;
        }

        /// <summary>
        /// Downloads the Report in binary form
        /// </summary>
        /// <returns></returns>
        public byte[] Download()
        {
            var url = this.GetUrl();
            var cl = new System.Net.WebClient();
            return cl.DownloadData(url);
        }

        /// <summary>
        /// Downloads the Report in binary form
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> DownloadAync()
        {
            var url = this.GetUrl();
            var cl = new System.Net.Http.HttpClient();
#pragma warning disable CA2234 // Pass system uri objects instead of strings
            return await cl.GetByteArrayAsync(url).ConfigureAwait(false);
#pragma warning restore CA2234 // Pass system uri objects instead of strings
        }

        private static string? GetStringFromArray(System.Collections.IEnumerable? input, string delimiter = ";")
        {
            if (input == null)
                return null;
            string output = "";
            foreach (var i in input)
            {
                if (i is DateTime dt)
                {
                    output += dt.ToString("yyyy-MM-dd") + delimiter;
                }
                else if (i is DateTimeOffset dto)
                {
                    output += dto.ToString("yyyy-MM-dd") + delimiter;
                }
                else
                {
                    output += i.ToString() + delimiter;
                }
            }
            if (output.EndsWith(delimiter))
                output = output.Substring(0, output.Length - delimiter.Length);
            return output;
        }

        /// <summary>
        /// Serializes an object to a string that can be used in a url
        /// </summary>
        public static string? GetStringOfValue(object? value)
        {
            if (value == null)
                return null;
            string? strValue;
            if (value is string s)
            {
                strValue = s;
            }
            else if (value is bool b)
            {
                strValue = b.ToString();
            }
            else if (value is int i)
            {
                strValue = i.ToString();
            }
            else if (value is byte bt)
            {
                strValue = bt.ToString();
            }
            else if (value is DateTime dt)
            {
                strValue = dt.ToString("yyyy-MM-dd");
            }
            else if (value is System.Collections.IEnumerable en)
            {
                strValue = GetStringFromArray(en);
            }
            else
            {
                strValue = value.ToString();
            }
            return strValue;

        }

        private void AddReportParameter(ref string url, string name, object? value)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            string paramPrefix = "";
            if (ReportServerURL != null && ReportServerURL.Contains("RSViewerPage.aspx")
                && !name.StartsWith("rs:")
                && !name.StartsWith("rc:")
                && !name.StartsWith("rp:"))
            {
                paramPrefix = "rp:";
            }
            if (url.Contains("?"))
            {
                url += "&";
            }
            else
            {
                url += "?";
            }

            if (value == null)
            {
                url += paramPrefix + name + ":IsNull=True";
                return;
            }
            else
            {
                string? strValue = GetStringOfValue(value)??"";
                url += String.Format("{0}={1}", paramPrefix + name, System.Uri.EscapeUriString(strValue));

            }

        }

        /// <summary>
        /// Gets the url of the report
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetUrl();
        }

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return GetUrl().GetHashCode();
        }
    }
}
