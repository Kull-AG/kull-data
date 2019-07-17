using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kull.Data.Test
{
    [TestClass]
    public class TimeZoneTest
    {
        [TestMethod]
        public void TestTimeZones()
        {
            string[] timeZones =
@"(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna
W. Europe Standard Time
(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague
UTC
(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna
Central Europe Standard Time
(GMT+01:00) Amsterdam, Berlin, Bern, Rom, Stockholm, Wien
GMT +0100 (Standard) / GMT +0200 (Daylight)
tzone://Microsoft/Utc
(GMT+01:00) West Central Africa
(GMT+01:00) Bruxelles, Berlin, Berne, Rome, Stockholm, Vienne
(UTC+01:00) Brussels, Copenhagen, Madrid, Paris
(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb
Europe/Berlin
(UTC+01:00) Amsterdam, Berlin, Bern, Rom, Stockholm, Wien
GMT +0100 (Standard) / GMT +0100 (Daylight)
(UTC+00:00) Dublin, Edinburgh, Lisbon, London
(UTC+01:00) West Central Africa
(GMT+01:00) Berlin, Bern, Brüssel, Rom, Stockholm, Wien
Europe/Zurich
(UTC+01:00) Amsterdam, Berlin, Berne, Rome, Stockholm, Vienne
(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague
GMT +0100 (Standard) / GMT +0200 (Daylight)  
GMT -0000 (Standard) / GMT +0100 (Daylight)".Replace("\r\n", "\n").Split('\n');
            foreach (var item in timeZones)
            {
                Kull.Data.TimezoneMapping.GetTimeZone(item);
            }

        }
    }
}
