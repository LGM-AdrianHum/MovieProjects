using System;
using System.IO;
using System.Linq;
using System.Net;
using TVDB.Model;
using TVDB.Web;

namespace UpdateEpisodeTables
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var o = 0;
            var db = new TvShows();
            var di = new DirectoryInfo(@"Q:\MyTv.Library");
            foreach (var ef in di.EnumerateFiles("tvdb_*.zip", SearchOption.AllDirectories))
            {
                var show = WebInterface.GetFullSeriesByFile(ef.FullName);
                var WorkingDirectory = ef.DirectoryName;
                Console.WriteLine($"Processing {ef.Name} - {show.Series.Name}");
                Console.Title = $"Processing {ef.Name} - {show.Series.Name}";
                var iid = GetTvDbId(show);
                var ls = db.Shows.FirstOrDefault(x => x.TvDbId == iid);
                if (ls == null)
                {
                    var sh = new Show
                    {
                        Name = show.Series.Name,
                        TvDbId = iid,
                        Status = show.Series.Status,
                        CreateDate = ef.CreationTime
                    };
                    db.Shows.Add(sh);
                }
                foreach (var ep in show.Series.Episodes)
                {
                    Console.WriteLine($"\tS{ep.SeasonNumber.ToString("00")}E{ep.Number.ToString("00")} - {ep.Name}");
                    o++;

                    var es = db.Episodes.FirstOrDefault(x => x.Id == ep.Id);
                    if (es == null)
                    {
                        var l = new Episode
                        {
                            EpisodeId = ep.Id,
                            EpisodeNumber = ep.Number,
                            SeasonNumber = ep.SeasonNumber,
                            FirstAired = ep.FirstAired,
                            Title = ep.Name,
                            TvDbId = iid
                        };
                        db.Episodes.Add(l);
                    }
                }
                db.SaveChanges();
                try
                {
                    var wc = new WebClient();
                    var k = show.Series.Banner;
                    if (string.IsNullOrEmpty(k)) return;
                    var u = new Uri($"http://thetvdb.com/banners/{k}");
                    var v = Path.Combine(WorkingDirectory,
                        "banner.jpg");
                    var v1 = Path.GetDirectoryName(v);
                    if (!Directory.Exists(v1)) if (v1 != null) Directory.CreateDirectory(v1);
                    Console.Write("\r\n.");
                    if (!File.Exists(v)) wc.DownloadFile(u, v);

                    k = show.Series.Poster;
                    u = new Uri($"http://thetvdb.com/banners/{k}");
                    v = Path.Combine(WorkingDirectory,
                        "Poster.jpg");
                    Console.Write(".");
                    if (!File.Exists(v)) wc.DownloadFile(u, v);

                    k = show.Banners.FirstOrDefault(x => x.Type == BannerTyp.fanart)?.BannerPath;
                    u = new Uri($"http://thetvdb.com/banners/{k}");
                    if (!string.IsNullOrEmpty(k))
                    {
                        v = Path.Combine(WorkingDirectory,
                            "Backdrop.jpg");
                        Console.Write(".");
                        if (!File.Exists(v)) wc.DownloadFile(u, v);
                    }

                    foreach (var vq in show.Banners)
                    {
                        var savepath = Path.Combine(WorkingDirectory, "extrafanart",
                            vq.BannerPath.Replace("/", "\\").TrimStart('\\'));
                        var pth = Path.GetDirectoryName(savepath);
                        if (!Directory.Exists(pth)) Directory.CreateDirectory(pth);
                        u = new Uri($"http://thetvdb.com/banners/{vq.BannerPath}");
                        Console.Write(".");
                        wc.DownloadFile(u, savepath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
                //if(show.Series?.Episodes == null) continue;
                //foreach (var vr in show.Series.Episodes)
                //{
                //    var savepath = Path.Combine(WorkingDirectory, "extrafanart",
                //        vr.PictureFilename.Replace("/", "\\").TrimStart('\\'));
                //    var pth = Path.GetDirectoryName(savepath);
                //    if (!Directory.Exists(pth)) Directory.CreateDirectory(pth);
                //    u = new Uri($"http://thetvdb.com/banners/{vr.PictureFilename}");
                //    Console.Write(".");
                //    wc.DownloadFile(u, savepath);
                //}
                Console.WriteLine(".");
            }
        }

        private static int GetTvDbId(SeriesDetails ret)
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