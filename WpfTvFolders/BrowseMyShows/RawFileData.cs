using System.Collections.Generic;
using System.IO;
using TvHelpers;

namespace BrowseMyShows
{
    public class RawFileData
    {
        public RawFileData(FileInfo f)
        {
            TvFileInfo = f;
            ParseValues = f.Name.TvFileTryParse();
            if (ParseValues == null) return;
            ShowName = ParseValues.Title;
            SeasonNumber = ParseValues.SeasonInt;
            EpisodeNumbers = ParseValues.Episodes;
        }

        public ParseValue ParseValues { get; set; }

        public FileInfo TvFileInfo { get; set; }

        public string ShowName { get; set; }
        public int SeasonNumber { get; set; }
        public List<int> EpisodeNumbers { get; set; }
    }
}