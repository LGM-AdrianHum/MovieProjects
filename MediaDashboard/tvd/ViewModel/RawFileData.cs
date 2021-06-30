using System.Collections.Generic;
using System.IO;
using System.Linq;
using PropertyChanged;
using TVDB.Utility;

namespace tvd.ViewModel
{
    [ImplementPropertyChanged]
    public class RawFileData
    {
        public RawFileData(FileInfo f)
        {
            TvFileInfo = f;
            ParseValues = f.Name.TvFileTryParse();
            ShowName = f.Name;
            if (ParseValues == null) return;
            ShowName = ParseValues.Title;
            SeasonNumber = ParseValues.SeasonInt;
            EpisodeNumbers = ParseValues.Episodes;
            EpisodeNumbersStr = string.Join("E", EpisodeNumbers.Select(x => x.ToString("00")));
            FullSeasonEpisode = $"S{SeasonNumber.ToString("00")}E{EpisodeNumbersStr}";
        }

        public string FullSeasonEpisode { get; set; }

        public string EpisodeNumbersStr { get; set; }

        public ParseValue ParseValues { get; set; }

        public FileInfo TvFileInfo { get; set; }

        public string ShowName { get; set; }
        public int SeasonNumber { get; set; }
        public List<int> EpisodeNumbers { get; set; }

        public override string ToString()
        {
            return ShowName;
        }
    }
}