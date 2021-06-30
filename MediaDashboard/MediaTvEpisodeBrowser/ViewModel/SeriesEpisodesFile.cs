using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PropertyChanged;
using TVDB.Model;
using UtilityFunctions;
using UtilityFunctions.Movie;

namespace MediaTvEpisodeBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class SeriesEpisodesFile
    {
        public SeriesEpisodesFile(FileInfo f)
        {
            Info = f;
            if (f.Exists)
            {
                FileName = f.Name;
                FileFullName = f.FullName;
                FileSizeInfo = f.Length.ToBytes();
                FileCreationDate = f.CreationTime;


                var pp = f.Name.TvFileTryParse();
                if (pp == null) return;
                FileShowname = pp.Title;
                FileEpisodeNumbers = pp.Episodes;
                FileSeasonNumber = pp.SeasonInt;
            }
        }

        public string FileFullName { get; set; }

        public string FileName { get; set; }

        public DateTime FileCreationDate { get; set; }

        public string FileSizeInfo { get; set; }

        public FileInfo Info { get; set; }
        public string FileShowname { get; set; }
        public List<int> FileEpisodeNumbers { get; set; }
        public int FileSeasonNumber { get; set; }

        public string InfoSeriesName { get; set; }

        public string InfoSeasonEpisodeNumbers { get; set; }

        public string InfoEpisodeNames { get; set; }

        public IEnumerable<Episode> InfoEpisodes { get; set; }

        public void SetEpisodeInfo(Series ser)
        {
            try
            {

                if (ser == null) throw new NullReferenceException("Series");
                InfoSeriesName = ser.Name;
                var episodes = ser.Episodes;
                if (FileEpisodeNumbers == null || !FileEpisodeNumbers.Any()) return;
                InfoEpisodes =
                    episodes.Where(x => x.SeasonNumber == FileSeasonNumber && FileEpisodeNumbers.Contains(x.Number))
                        .OrderBy(x => x.Number);
                if (InfoEpisodes != null && InfoEpisodes.Any())
                {
                    InfoEpisodeNames = string.Join(", ", InfoEpisodes.Select(x => x.Name));
                    var firstOrDefault = InfoEpisodes.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        InfoSeasonEpisodeNumbers = "S" + firstOrDefault.SeasonNumber.ToString("00") + "E" +
                                                   string.Join("E", InfoEpisodes.Select(x => x.Number.ToString("00")));
                        InfoOverviews = string.Join("\r\n\r\n", InfoEpisodes.Select(x => x.Overview));
                        if (string.IsNullOrEmpty(InfoOverviews.Replace("\r", "").Replace("\n", ""))) InfoOverviews = "";

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string InfoOverviews { get; set; }
    }
}