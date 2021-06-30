using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PropertyChanged;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class ItemBase
    {

        public ItemBase()
        {
        }

        public ItemBase(string s)
        {
            FullName = s;
            Name = Path.GetFileName(s);
            Episodes = new List<int>();
        }

        public bool Success { get; set; }
        public int TvDbId { get; set; }
        public List<int> Episodes { get; set; }
        public int Episode { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string SearchKey { get; set; }
        public int Season { get; set; }
        public string SeasonEpisode { get; set; }
        public LibraryDirectory MatchingShow { get; set; }

        public string TargetDirectoryFullname => MatchingShow.FullName;

        public bool ParseWithRegex(Regex rx)
        {
            var link = rx.Match(Name);
            if (!link.Success)
            {
                var plr = Path.GetFileName(Path.GetDirectoryName(FullName));
                if (string.IsNullOrEmpty(plr))
                {
                    Success = false;
                    return Success;
                }
                link = rx.Match(plr);
                if (!link.Success)
                {
                    Success = link.Success;
                    return Success;
                }
            }
            Success = true;
            var season = link.Groups["season"].Value.Substring(1);
            var showname = link.Groups["showname"].Value;
            var ltr = " -.".ToCharArray();
            SearchShowName = showname.TrimEnd(ltr).Replace(".", " ");
            var i = 0;
            if(int.TryParse(season,out i)) Season = i; else Season=-1;

            var rgx = new Regex("[^a-zA-Z0-9]");
            SearchKey = rgx.Replace(showname, "").ToLower();
            foreach (Capture cap in link.Groups["episode"].Captures)
            {
                i = 0;
                if(int.TryParse(cap.Value.Substring(1), out i)) Episodes.Add(i);
            }
            Episode = Episodes.FirstOrDefault();
            SeasonEpisode = $"S{Season.ToString("00")}E" + string.Join("E",Episodes.Select(x=>x.ToString("00")));
            return Success;
        }

        public string SearchShowName { get; set; }

        public string TvShowName => MatchingShow?.Name;

        public string EpisodeName
        {
            get
            {
                if (MatchingShow?.SeriesInformation?.Series?.Episodes == null || !MatchingShow.SeriesInformation.Series.Episodes.Any()) return "";
                
                var ep = MatchingShow.SeriesInformation.Series.Episodes.Where(x => x.SeasonNumber == Season
                                                                                   && x.Number == Episode);

                return ep.FirstOrDefault()?.Name;
            }
        }
    }
}