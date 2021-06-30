using System;
using System.Collections.Generic;

namespace TvFileBot.ViewModel
{
    public static class LongToBytesExtension
    {
        private const int Precision = 2;

        private static readonly IList<string> Units;

        static LongToBytesExtension
            ()
        {
            Units = new List<string>
            {
                "B",
                "KB",
                "MB",
                "GB",
                "TB"
            };
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
    }
}