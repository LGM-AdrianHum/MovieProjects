using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PropertyChanged;
using TVDB.Model;
using TVDB.Web;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class ShowDirectory
    {
        public ShowDirectory(string a, string x)
        {
            FullName = x;
            Name = Path.GetFileName(x);
            DirNumber = a;
            var te = new TextBlock();
            DirectoryColor = Directory.GetFiles(x, "tvdb_*.zip").Any()
                ? new SolidColorBrush(Colors.Black)
                : new SolidColorBrush(Colors.DarkGreen);
        }

        public string PresentName => $"[{DirNumber}] {Name}";

        public SolidColorBrush DirectoryColor { get; set; }


        public string FullName { get; set; }
        public string Name { get; set; }

        public string DirNumber { get; set; }
        public SeriesDetails SeriesInfo { get; set; }

        public int TvDbId { get; set; }


        public string BannerPath { get; set; }


        public string FullBannerPath
            => string.IsNullOrEmpty(BannerPath) ? "" : $"http://thetvdb.com/banners/{BannerPath}";

        public string FanArtPath { get; set; }


        public string FullFanArt => string.IsNullOrEmpty(FanArtPath) ? "" : $"http://thetvdb.com/banners/{FanArtPath}";

        public string PosterPath { get; set; }

        public string FullPosterPath
            => string.IsNullOrEmpty(PosterPath) ? "" : $"http://thetvdb.com/banners/{PosterPath}";

        public SeriesDetails FromShowDetails { get; set; }

        public override string ToString()
        {
            return PresentName;
        }

        public bool Load()
        {
            //var dc = App.Current.MainWindow.DataContext as DirectoryManagerVm;


            if (!Directory.Exists(FullName)) return false;
            var a = Directory.GetFiles(FullName, "tvdb_*.zip").FirstOrDefault();
            if (a == null) return false;
            SeriesInfo = WebInterface.GetFullSeriesByFile(a);

            if (SeriesInfo == null) return false;

            TvDbId = SeriesInfo.Series.SeriesId;
            if (TvDbId < 1) TvDbId = SeriesInfo.Id;
            if (TvDbId < 1) TvDbId = SeriesInfo.Series.Id;
            if (string.IsNullOrEmpty(SeriesInfo.Series.Banner))
            {
                var firstbanner = SeriesInfo?.Banners?.FirstOrDefault();
                if (firstbanner != null)
                {
                    SeriesInfo.Series.Banner = firstbanner.BannerPath;
                    Save(TvDbId.ToString());
                }
            }
            BannerPath = SeriesInfo.Series.Banner;
            PosterPath = SeriesInfo.Series.Poster;
            return true;
        }

        public async void Save(string tvDbId)
        {
            var i = 0;
            if (!int.TryParse(tvDbId, out i)) return;
            var cl = new WebInterface("", FullName);
            var mm = await cl.GetMirrors();
            if (mm != null && mm.Any())
            {
                var mmm = mm.FirstOrDefault();
                var cc = cl.SaveFullSeriesById(FullName, i, mmm);
                if (cc != null)
                {
                    FromShowDetails = cc;
                    TvDbId = cc.Series.Id;
                    PosterPath = cc.Series.Poster;
                }
            }
        }
    }
}