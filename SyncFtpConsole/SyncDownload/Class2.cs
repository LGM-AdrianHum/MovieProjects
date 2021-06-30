using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SyncDownload
{
    public class BytesToStringConverter : System.Windows.Data.IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as Double? ?? 0).ToBytes();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
            value = value.TruncateToSignificantDigits(Precision);
            var frmtr = value.ToString("0.0000").Substring(0, 4).TrimEnd('.', ' ');
            return frmtr + " " + Units[(int)pow];
        }
        static double TruncateToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1 - digits);
            return scale * Math.Truncate(d / scale);
        }
        public static string ToBytes(this int bytes)
        {
            return Convert.ToInt64(bytes).ToBytes();
        }

        public static string ToBytes(this double bytes)
        {
            return Convert.ToInt64(bytes).ToBytes();
        }


    }

    public static class FileTypeFilters
    {
        public static bool IsMovie(this string data)
        {
            var allvideotype = ".avi|.iso|.m4v|.mkv|.mp4|.mpg|.vob".Split('|');
            if (allvideotype.Contains(data.ToLower())) return true; // extension only handler.
            var ext = Path.GetExtension(data);
            return allvideotype.Contains(ext.ToLower());
        }

        public static bool IsMovie(this FileInfo data)
        {
            return data != null && data.Extension.IsMovie();
        }


        public static bool IsBitmap(this string data)
        {
            var allpicturetypes = ".png|.jpg|.jpeg|.bmp|.gif".Split('|');
            if (allpicturetypes.Contains(data.ToLower())) return true; // extension only handler.
            var ext = Path.GetExtension(data);
            return allpicturetypes.Contains(ext.ToLower());
        }
        public static bool IsBitmap(this FileInfo data)
        {
            return data != null && data.Extension.IsBitmap();
        }
    }


}

