using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TVDB.Utility;

namespace UtilityFunctions.Movie
{
    public static class MovieNameHelper
    {
        public static ParseValue MovieParse(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            const string pattern = @"
                            ^(?<Name>.+?)                  # Movie Name up to year and resolution
                            (?!\.[12]\d\d\d\.\d{,3}[ip]\.) # Year and resolution foward negative look ahead as an a pattern anchor
                            \.                             # Non captured due to only explicitly capturing.
                            (?<Year>\d\d\d\d)              # Capture Year, etc...
                            \.
                            (?<Resolution>[^.]+)
                            \.
                            (?<Format>[^.]+)
                            ";

            var list =
                Regex.Matches(data, pattern,
                    RegexOptions.ExplicitCapture // Only what we ask for (?<> ), ignore non captures
                    | RegexOptions.Multiline // ^ makes each line a separate one.
                    | RegexOptions.IgnorePatternWhitespace) // Allows us to comment pattern only.
                    .OfType<Match>().Select(mt => new ParseValue
                    {
                        Movie = Regex.Replace(mt.Groups["Name"].Value, @"\.", " "),
                        Year = mt.Groups["Year"].Value,
                        Resolution = mt.Groups["Resolution"].Value,
                        Format = mt.Groups["Format"].Value
                    }).FirstOrDefault();
            return list;
        }

        public static ParseValue MovieParse2(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var rx = new Regex(@"^(?<Name>.+)\((?<Year>\d+)\)(?<AdditionalText>[^\.]*)");
            var mt = rx.Match(data);
            if (mt.Success)
            {
                return new ParseValue
                {
                    Movie = Regex.Replace(mt.Groups["Name"].Value, @"\.", " ").Trim(),
                    Year = mt.Groups["Year"].Value.Trim(),
                    AdditionalText = mt.Groups["AdditionalText"].Value.Trim()
                };
            }
            return null;
        }

        public static ParseValue MovieParse3(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var rx = new Regex(@"(?<Name>.+).(?<Year>\d{4})");
            var mt = rx.Match(data);
            if (mt.Success)
            {
                return new ParseValue
                {
                    Movie = Regex.Replace(mt.Groups["Name"].Value, @"\.", " ").Trim(),
                    Year = mt.Groups["Year"].Value.Trim(),
                    AdditionalText = ""
                };
            }
            return null;
        }


    }
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
