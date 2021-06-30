using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using PropertyChanged;
using TVDB.Model;
using TVDB.Web;
using UtilityFunctions;
using UtilityFunctions.Movie;

namespace MediaTvEpisodeBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class TvEpisodeFolder
    {
        public readonly DirectoryInfo Di;
        private string _datafile;


        public TvEpisodeFolder(DirectoryInfo d)
        {
            if (d != null)
            {
                Di = d;

                Name = Di.Name;

                _datafile = GetDataFile();

                BannerName = File.Exists(Path.Combine(Di.FullName, "banner.jpg"))
                    ? Path.Combine(Di.FullName, "banner.jpg")
                    : "";
            }
        }

        public string BannerName { get; set; }

        public SeriesDetails TvSeriesDetails { get; set; }
        public ObservableCollection<EpFile> AllEpisodes { get; set; }

        public SolidColorBrush StatusColor { get; set; }
        public int TvDbId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }

        private string GetDataFile()
        {
            var v = Di.GetFiles("tvdb_*.zip");
            if (v.Length > 1) Process.Start(Di.FullName);
            return v.FirstOrDefault()?.FullName;
        }

        public void SetData()
        {
            TvSeriesDetails = LoadData();
        }


        private SeriesDetails LoadData()
        {
            _datafile = GetDataFile();
            if (string.IsNullOrEmpty(_datafile))
                return null;

            var ret = WebInterface.GetFullSeriesByFile(_datafile);
            if (ret == null)
            {
                TvDbId = -1;
                return null;
            }


            TvDbId = GetTvDbId(ret);
            Status = "Lost";
            if (!string.IsNullOrEmpty(ret.Series?.Status)) Status = ret.Series.Status;

            switch (Status.Substring(0, 1))
            {
                case "C":
                    {
                        StatusColor = new SolidColorBrush(Colors.Green);
                        break;
                    }
                case "E":
                    {
                        StatusColor = new SolidColorBrush(Colors.Orange);
                        break;
                    }
                default:
                    {
                        StatusColor = new SolidColorBrush(Colors.OrangeRed);
                        break;
                    }
            }

            try
            {
                if (ret.Series?.Episodes != null && ret.Series.Episodes.Count > 0)
                {

                    var af =
                        Di.GetFiles("*.*", SearchOption.AllDirectories)
                            .AsParallel()
                            .Where(x => x.Extension.IsMovie())
                            .ToList();

                    var k1 = af.AsParallel().Select(x => new SeriesEpisodesFile(x)).ToList();
                    foreach (var k in k1)
                    {
                        k.SetEpisodeInfo(ret.Series);
                    }

                    var showName = ret.Series != null ? ret.Series.Name : "Lost Show Name";


                    var ae = ret.Series.Episodes.Select(x => new EpFile(x, showName)).ToList();

                    var extra = new List<EpFile>();


                    if (extra.Any()) ae.AddRange(extra);
                    AllFiles = new ObservableCollection<SeriesEpisodesFile>(k1.OrderBy(x => x.InfoSeasonEpisodeNumbers));
                    AllEpisodes = new ObservableCollection<EpFile>(ae.OrderBy(x => x.SeasonEpisode));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return ret;
        }

        public ObservableCollection<SeriesEpisodesFile> AllFiles { get; set; }

        private int GetTvDbId(SeriesDetails ret)
        {
            if (ret?.Series == null) return -1;
            var id = ret.Series.SeriesId;
            if (id < 1) id = ret.Id;
            if (id < 1) id = ret.Series.Id;
            if (id < 1) id = -1;
            return id;
        }
    }


}