using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kull.Data.Reporting
{
    /// <summary>
    /// A Helper class for Report Formats such as PDF, Excel and more
    /// </summary>
    public class ReportFormat
    {
        /// <summary>
        /// An array containing default formats
        /// 
        /// </summary>
        public static readonly ReportFormat[] DefaultFormats
                = new ReportFormat[]
                {
                     new ReportFormat("PDF", "PDF", ".pdf"),
                     new ReportFormat("Excel", "EXCELOPENXML", ".xlsx"),
                     new ReportFormat("Html", null, ".html"),
                     new ReportFormat("Webarchiv", "MHTML", ".mhtml"),
                     new ReportFormat("Word", "WORDOPENXML", ".docx")
                };


        /// <summary>
        /// The PDF Format
        /// </summary>
        public static ReportFormat PDF { get { return DefaultFormats[0]; } }

        /// <summary>
        /// The Excel Format
        /// </summary>
        public static ReportFormat Excel { get { return DefaultFormats[1]; } }

        /// <summary>
        /// The Excel Format, Version 2003
        /// </summary>
        public static readonly ReportFormat Excel2003 = new ReportFormat("Excel", "EXCEL", ".xls");

        /// <summary>
        /// The Html Format
        /// </summary>
        public static ReportFormat Html { get { return DefaultFormats[2]; } }

        /// <summary>
        /// The Webarchive Format, measing a file containing all required files
        /// </summary>
        public static ReportFormat Webarchiv { get { return DefaultFormats[3]; } }

        /// <summary>
        /// The MS Word Format
        /// </summary>
        public static ReportFormat Word { get { return DefaultFormats[4]; } }

        /// <summary>
        /// The MS Word 2003 Format
        /// </summary>
        public static readonly ReportFormat Word2003 = new ReportFormat("Word", "WORD", ".doc");

        /// <summary>
        /// Creates a new Reportformat
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="rsParam"></param>
        /// <param name="extension"></param>
        public ReportFormat(string displayName, string? rsParam, string extension)
        {
            this.RSParam = rsParam;
            this.DisplayName = displayName;
            this.Extension = extension;
        }

        /// <summary>
        /// Creates a new Report Format
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="extension"></param>
        public ReportFormat(string displayName, string extension)
            : this(displayName, displayName, extension)
        {
        }

        /// <summary>
        /// The Parameter for the report server
        /// </summary>
        public readonly string? RSParam = "";

        /// <summary>
        /// A Friendly name
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// The extension
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ReportFormat)
                return Equals((ReportFormat)obj);
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a Integer unique per format (checks RSParam field)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.RSParam??"").GetHashCode();
        }

        /// <summary>
        /// Checks two formats for equality
        /// </summary>
        /// <param name="otherReportFormat"></param>
        /// <returns></returns>
        public bool Equals(ReportFormat otherReportFormat)
        {
            return otherReportFormat.RSParam == this.RSParam;
        }
    }
}
