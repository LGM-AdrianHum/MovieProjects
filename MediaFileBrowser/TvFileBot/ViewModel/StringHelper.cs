using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TvFileBot.ViewModel
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
}