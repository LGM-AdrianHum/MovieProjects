using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EpisodeViewer
{
    public static class StringHelper
    {
        public static string ToCleanString(this string s)
        {
            var invalidPathChars = Path.GetInvalidFileNameChars();
            var cly = s.Replace(":", ",");
            cly = invalidPathChars.Aggregate(cly, (current, rr) => current.Replace(rr, ' '));
            var myTi = new CultureInfo("en-US", false).TextInfo;
            return Regex.Replace(cly, @"\s+", " ").TrimEnd().TrimStart();
        }
    }
    public static class NumberToBytes
    {

        private const int Precision = 3;

        private static readonly IList<string> Units;

        static NumberToBytes()
        {
            Units = "B|KB|MB|GB|TB".Split('|').ToList();
        }

        /// <summary>
        ///     Formats the value as a filesize in bytes (KB, MB, etc.)
        /// </summary>
        /// <param name="bytes">This value.</param>
        /// <returns>Filesize and quantifier formatted as a string.</returns>
        public static string ToBytes(this long bytes)
        {
            var pow = Math.Floor((bytes > 0 ? Math.Log(bytes) : 0) / Math.Log(1024));
            pow = Math.Min(pow, Units.Count - 1);
            var value = bytes / Math.Pow(1024, pow);
            return value.ToString(Math.Abs(pow) < 0.001 ? "F0" : "F" + Precision) + " " + Units[(int)pow];
        }

        public static string ToBytes(this int bytes)
        {
            return Convert.ToInt64(bytes).ToBytes();
        }
    }
}