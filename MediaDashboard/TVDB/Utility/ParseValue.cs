using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PropertyChanged;
using TVDB.Model;

namespace TVDB.Utility
{
    /// <summary>
    /// </summary>
    [Serializable]
    [ImplementPropertyChanged]
    public class ParseValue
    {
        /// <summary>
        /// </summary>
        public string MovieYear => string.IsNullOrEmpty(Year) ? Movie : $"{Movie} ({Year})";

        /// <summary>
        /// </summary>
        public string Movie { get; set; }

        /// <summary>
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// </summary>
        public string AdditionalText { get; set; }

        /// <summary>
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// </summary>
        public string SeasonNumber { get; set; }

        /// <summary>
        /// </summary>
        public List<int> Episodes { get; set; }

        /// <summary>
        /// </summary>
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// </summary>
        public int SeasonInt { get; set; }

        /// <summary>
        /// </summary>
        public FileInfo MediaFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SeasonEpisode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TitleSeasonEpisode { get; set; }

        /// <summary>
        /// </summary>
        public string NewFileName { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(MovieYear) ? Title + " " + SeasonEpisode : MovieYear;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ParseTv()
        {
            if (MediaFile == null) return;
            var rx =
                new Regex(
                    @"^(?<showname>.+?).([Ss]?)(?<seasonnumber>[0-9]{1,2})([xXeE](?<episodenamenumber>[0-9]{1,2})?){1,99}");
            var mt = rx.Match(MediaFile.Name);

            while (!mt.Success) return;


            Title = Regex.Replace(mt.Groups["showname"].Value.TrimEnd('-', ' '), @"\.", " ");
            SeasonNumber = mt.Groups["seasonnumber"].Value;
            Episodes = new List<int>();


            var eppi = mt.Groups["episodenamenumber"].Captures.Cast<Capture>().Select(x => x.Value).Distinct().ToList();

            foreach (var eppii in eppi)
            {
                int ii;
                if (int.TryParse(eppii, out ii)) Episodes.Add(ii);
            }

            if (!Episodes.Any())
            {
                Episodes = null;

                return;
            }
            EpisodeNumber = Episodes.FirstOrDefault();

            int k;
            if (int.TryParse(SeasonNumber, out k)) SeasonInt = k;

            TitleSeasonEpisode =
                $"{Title} - S{SeasonInt.ToString("00")}E{string.Join("E", Episodes.Select(x => x.ToString("00")))}";
            SeasonEpisode =
                $"S{SeasonInt.ToString("00")}E{string.Join("E", Episodes.Select(x => x.ToString("00")))}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sd"></param>
        public void SetFilename(SeriesDetails sd)
        {
            if (sd.Series == null) return;

            var n1 = sd.Series.Name.ToCleanString();
            var n2 = SeasonEpisode;

            var n4 =
                string.Join(", ",
                    sd.Series.Episodes.Where(x => x.SeasonNumber == SeasonInt && Episodes.Contains(x.Number))
                        .Select(x => x.Name.ToCleanString()));

            NewFileName = $"{n1} - {n2} - {n4}{MediaFile.Extension}";
        }
    }
}