using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TVDB.Utility
{
    public static class FileNameHelpers
    {
        public static string ToCleanString(this string s)
        {
            var invalidPathChars = Path.GetInvalidFileNameChars();
            var cly = s.Replace(":", ",");
            cly = invalidPathChars.Aggregate(cly, (current, rr) => current.Replace(rr, ' '));
            var myTi = new CultureInfo("en-US", false).TextInfo;
            return Regex.Replace(cly, @"\s+", " ").TrimEnd().TrimStart();
        }

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

        public static List<DirectoryInfo> GetDirectories(DirectoryInfo dirName)
        {
            var f1 = new List<DirectoryInfo>();
            for (var i = 1; i < 10; i++)
            {
                var dn = new DirectoryInfo(Path.Combine(dirName.FullName, i.ToString("000")));
                if (!dn.Exists) continue;
                var b =
                    dn.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly)
                        .AsParallel()
                        .Select(x => x);
                f1.AddRange(b);
            }
            return f1;
        }

      

        public static List<FileInfo> GetFiles(DirectoryInfo dirName)
        {
            var f1 = new List<FileInfo>();
            var b =
                dirName.EnumerateFiles("*.*", SearchOption.AllDirectories)
                    .AsParallel()
                    .Where(x => x.Name.IsMovie())
                    .Select(x => x);
            f1.AddRange(b);
            return f1;
        }

        public static ParseValue TvFileTryParse(this string s)
        {
            var prr = new ParseValue();

            var rx =
                new Regex(
                    @"^(?<showname>.+?).([Ss]?)(?<seasonnumber>[0-9]{1,2})([xXeE](?<episodenamenumber>[0-9]{1,2})?){1,99}");
            var mt = rx.Match(s);

            while (!mt.Success) return null;

            var pr = new ParseValue
            {
                Title = Regex.Replace(mt.Groups["showname"].Value.TrimEnd('-', ' '), @"\.", " "),
                SeasonNumber = mt.Groups["seasonnumber"].Value,
                Episodes = new List<int>()
            };

            var eppi = mt.Groups["episodenamenumber"].Captures.Cast<Capture>().Select(x => x.Value).Distinct().ToList();

            foreach (var eppii in eppi)
            {
                var ii = 0;
                if (int.TryParse(eppii, out ii)) pr.Episodes.Add(ii);
            }

            if (!pr.Episodes.Any())
                pr.Episodes = null;
            else
                pr.EpisodeNumber = pr.Episodes.FirstOrDefault();

            var k = 0;
            if (int.TryParse(pr.SeasonNumber, out k)) pr.SeasonInt = k;

            return pr;
        }
    }
}