using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kull.Data
{
    /// <summary>
    /// A class for mapping Oslon Style Time zones to Windows Style Timezones
    /// </summary>
    public static class TimezoneMapping
    {

        internal static readonly Dictionary<string, string> TimeZoneAliases = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            { "GMT +0100 (Standard) / GMT +0200 (Daylight)", "W. Europe Standard Time" },
            { "GMT -0000 (Standard) / GMT +0100 (Daylight)", "GMT Standard Time" },
            { "UTC +0100 (Standard) / GMT +0100 (Daylight)" , "W. Central Africa Standard Time"},
            {  "GMT +0100 (Standard) / GMT +0100 (Daylight)" , "W. Central Africa Standard Time" },
            {"(UTC+01:00) West Central Africa", "W. Central Africa Standard Time" },
            { "Romance Standard Time", "Central Europe Standard Time" }
        };



        /// <summary>
        /// Convert a text timezone to a TimeZoneInfo
        /// </summary>
        /// <param name="value">The timezone to convert</param>
        /// <returns>The Timezoneinfo object</returns>
        public static TimeZoneInfo GetTimeZone(string value)
        {

            if (value.StartsWith("(GMT"))
            {
                return GetTimeZone("(UTC" + value.Substring("(GMT".Length));
            }
            if ("tzone://Microsoft/Utc".Equals(value.Trim(), StringComparison.CurrentCultureIgnoreCase) ||
                "UTC".Equals(value.Trim(), StringComparison.CurrentCultureIgnoreCase))
                //For what ever reason this value is not recognized as a timezone
                return TimeZoneInfo.Utc;
            if (TimeZoneAliases.TryGetValue(value.Trim(), out var alias))
            {
                return GetTimeZone(alias);
            }
            try
            {
#if NET6_0 // In .Net 6 unix id's should work

                if (System.Environment.OSVersion.Platform == PlatformID.Win32NT && TimeZoneInfo.TryConvertIanaIdToWindowsId(value.Trim(), out string? winId))
                    return TimeZoneInfo.FindSystemTimeZoneById(winId);
                if (System.Environment.OSVersion.Platform == PlatformID.Unix && TimeZoneInfo.TryConvertWindowsIdToIanaId(value.Trim(), out string? ianaId))
                    return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
                return TimeZoneInfo.FindSystemTimeZoneById(value.Trim());
#else
                return TimeZoneConverter.TZConvert.GetTimeZoneInfo(value.Trim());
#endif
            }
            catch (System.TimeZoneNotFoundException)
            {
                var sysTimeZones = System.TimeZoneInfo.GetSystemTimeZones();
                var utcOffset = value.StartsWith("(UTC") ? TimeSpan.Parse(value.Substring("(UTC+".Length-1, "+01:00".Length).Replace("+", "")) : (TimeSpan?)null;
                var towns = value.Contains(" ") ? value.Substring(value.IndexOf(" ")).Split(',').Select(s => s.Trim()).ToArray() : Array.Empty<string>();
                foreach (var st in sysTimeZones)
                {
                    if (st.DisplayName.Equals(value.Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        return st;
                    }
                    if (System.Environment.OSVersion.Platform == PlatformID.Unix && st.Id.Contains("/"))
                    {
                        var ianaTown = st.Id.Split('/')[1];
                        if (st.BaseUtcOffset == utcOffset && towns.Contains(ianaTown, StringComparer.OrdinalIgnoreCase))
                        {
                            return st;
                        }
                    }
                    else
                    {
                        int utcOffSetDisplayIndex = st.DisplayName.IndexOf(")");
                        string[] towns1 = st.DisplayName.Substring(utcOffSetDisplayIndex + 1).Trim().Split(',').Select(s => s.Trim()).ToArray();
                        if (st.BaseUtcOffset == utcOffset && towns1.Any(t => towns.Contains(t)))
                        {
                            return st;
                        }
                    }
                }
                throw;
            }
        }

    }

}
