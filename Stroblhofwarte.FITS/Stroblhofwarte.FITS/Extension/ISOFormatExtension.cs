using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.FITS.Extension
{
    public static class ISOFormatExtensions
    {
        const string ISOFORMAT = "yyyy-MM-dd\\THH:mm:ss.fffK"; //ISO-8601 used by Javascript (ALWAYS UTC)
        public static string toISOString(this DateTime d, bool useLocal = false)
        {
            if (!useLocal && d.Kind == DateTimeKind.Local)
            {
                //If d is LT or you don't want LocalTime -> convert to UTC and always add K format always add 'Z' postfix
                return d.ToUniversalTime().ToString(ISOFORMAT);
            }
            else
            { //If d is already UTC K format add 'Z' postfix, if d is LT K format add +/-TIMEOFFSET
                return d.ToString(ISOFORMAT);
            }
        }
        public static DateTime fromISOString(this DateTime d, string s, bool useLocal = false)
        {
            //Return a new DateTime parsed used ISOFORMAT - YOU MUST PASS A STRING ENDING WITH 'Z' OR +/-TIMEOFFSET
            var l = DateTime.ParseExact(s, ISOFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            return useLocal ? l : l.ToUniversalTime(); //If you don't set useLocal returned date is always Kind=UTC
        }
        public static DateTime fromISOString(this DateTime d, string date, string time, bool useLocal = false)
        {
            //Return a new DateTime buiding an ISOFROMAT string from date, time params expressed in UTC (by default) or in LT if you set useLocal=true 
            var sb = new System.Text.StringBuilder(30);
            if (!string.IsNullOrEmpty(date)) { sb.Append(date); sb.Replace('-', '/'); }
            if (!string.IsNullOrEmpty(time)) { sb.Append(' '); sb.Append(time); }
            var s = sb.ToString();
            if (!useLocal)
            { //Always return DateTime Kind=UTC, if you don't pass +/-TIMEOFFSET or 'Z' postfix I'll add it by default (as needed for UTC)
                if (!(s.Contains('Z') || s.Contains('+') || s.Contains('-'))) s += "Z";
                return d = DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal);
            }
            else
            { //Return DateTime Kind=Local and do necessary conversion to LT if you pass time with +/-TIMEOFFSET or referred as UTC with 'Z' postfix
                return d = DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
            }
        }
    }
}
