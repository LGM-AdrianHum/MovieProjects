using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TVDB.Utility;

namespace UtilityFunctions.Movie
{
    public static class TvSeriesNameHelper
    {
        public static ParseValue TvFileTryParse(this string s)
        {
            var prr = new ParseValue();
            
            var rx =
                new Regex(
                    @"^(?<showname>.+?).([Ss]?)(?<seasonnumber>[0-9]{1,2})([xXeE](?<episodenamenumber>[0-9]{1,2})?){1,99}");
            var mt = rx.Match(s);

            if (!mt.Success) return null;

            var pr = new ParseValue
            {
                Title = Regex.Replace(mt.Groups["showname"].Value.TrimEnd('-', ' '), @"\.", " "),
                SeasonNumber = mt.Groups["seasonnumber"].Value,
                Episodes = new List<int>(),
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
