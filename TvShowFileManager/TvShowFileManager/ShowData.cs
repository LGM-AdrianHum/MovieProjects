using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TVDB.Model;
using TVDB.Web;

namespace TvShowFileManager
{
    public class ShowData
    {
        private Series _series;

        public ShowData(Series series, int id)
        {
            _series = series;

            ShowDataId = series.SeriesId;
            ShowName = series.Name;
            ShowEpisodes = series.Episodes;
        }

        public ShowData(int id)
        {
            ShowDataId = id;
        }

        public ICollection<DirInfo> StorageDirectories { get; set; }
        public int ShowDataId { get; set; }
        public ObservableCollection<TVDB.Model.Episode> ShowEpisodes { get; set; }
        public SeriesDetails SeriesInformation { get; set; }
        public string ShowName { get; set; }

        public async void GetTvDbDetails()
        {
            if (ShowDataId == 0) return;
            if (SeriesInformation != null || ShowDataId == -1) return;
            var client = new WebInterface("", @"K:\\CacheData");
            var mirrors = await client.GetMirrors();
            var mirror = mirrors.FirstOrDefault();
            if (mirror == null) return;
            var dt = await client.GetFullSeriesById(ShowDataId, mirror);
            if (dt == null)
            {
                ShowDataId = -1;
                return;
            }
            SeriesInformation = dt;
            ShowDataId = dt.Id;
        }
    }
}