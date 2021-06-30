using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using PropertyChanged;
using Relays;
using TVDB.Model;
using TVDB.Web;

namespace UpdateTvDetails
{
    [ImplementPropertyChanged]
    public class ViewModel
    {
        public ViewModel()
        {
            GoogleSearch = new RelayCommand(DoGoogleSearch, o => true);
            TvDbSearch = new RelayCommand(DoTvDbSearch, o => true);
            AcceptData = new RelayCommand(DoAcceptData, o => true);
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                WorkingDirectory = args[1].TrimEnd('\\').TrimEnd('\"');
                QueryString = Path.GetFileName(WorkingDirectory);
            }
        }

        public string WorkingDirectory { get; set; }

        private async void DoAcceptData(object obj)
        {
            IsBusy = true;
            await AnAsyncMethodThatCompletes();
            App.Current.Shutdown();
        }

        public bool IsBusy { get; set; }

        public async Task AnAsyncMethodThatCompletes()
        {


            if (GoogleSelected == null) return;
            if (!Directory.Exists(WorkingDirectory)) return;
            var client = new WebInterface(null, "q:\\");
            var mirs = await client.GetMirrors();
            if (mirs == null || !mirs.Any()) return;
            var m = mirs.FirstOrDefault();
            if (m == null) return;
            var id = 0;
            if (int.TryParse(GoogleSelected.Id, out id)) SeriesResult = client.SaveFullSeriesById(WorkingDirectory, id, m);

            var wc = new WebClient();
            var k = SeriesResult.Series.Banner;
            if (string.IsNullOrEmpty(k)) return;
            var u = new Uri($"http://thetvdb.com/banners/{k}");
            var v = Path.Combine(WorkingDirectory,
                "banner.jpg");
            var v1 = Path.GetDirectoryName(v);
            if (!Directory.Exists(v1)) if (v1 != null) Directory.CreateDirectory(v1);

            if (!File.Exists(v)) await wc.DownloadFileTaskAsync(u, v);

            k = SeriesResult.Series.Poster;
            u = new Uri($"http://thetvdb.com/banners/{k}");
            v = Path.Combine(WorkingDirectory,
                "Poster.jpg");
            if (!File.Exists(v)) await wc.DownloadFileTaskAsync(u, v);

            k = SeriesResult.Banners.FirstOrDefault(x=>x.Type== BannerTyp.fanart)?.BannerPath;
            u = new Uri($"http://thetvdb.com/banners/{k}");
            if (!string.IsNullOrEmpty(k))
            {
                v = Path.Combine(WorkingDirectory,
                    "Backdrop.jpg");
                if (!File.Exists(v)) await wc.DownloadFileTaskAsync(u, v);
            }

            foreach (var vq in SeriesResult.Banners)
            {
                var savepath = Path.Combine(WorkingDirectory, "extrafanart",
                    vq.BannerPath.Replace("/", "\\").TrimStart('\\'));
                var pth = Path.GetDirectoryName(savepath);
                if (!Directory.Exists(pth)) Directory.CreateDirectory(pth);
                u = new Uri($"http://thetvdb.com/banners/{vq.BannerPath}");
                await wc.DownloadFileTaskAsync(u, savepath);
            }

            await Task.Factory.StartNew(() => { }); // <-- This line here, at the end
        }

        public SeriesDetails SeriesResult { get; set; }

        public RelayCommand AcceptData { get; set; }

        public RelayCommand TvDbSearch { get; set; }

        public RelayCommand GoogleSearch { get; set; }

        public string QueryString { get; set; }

        public ObservableCollection<GoogleResult> AllGoogleResult { get; set; }

        public GoogleResult GoogleSelected { get; set; }

        private async void DoTvDbSearch(object obj)
        {
            try
            {
                var client = new WebInterface(null, @"q:\");

                var mirs = await client.GetMirrors();
                if (mirs == null || !mirs.Any()) return;
                var m = mirs.FirstOrDefault();
                var search = await client.GetSeriesByName(QueryString, m);
                if (search == null) return;
                AllGoogleResult = new ObservableCollection<GoogleResult>(
                    search.Select(x => new GoogleResult(x))
                        .Where(
                            x =>
                                !string.IsNullOrEmpty(x.Id) && !string.IsNullOrEmpty(x.Title))
                        .OrderBy(x => x.Title).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void DoGoogleSearch(object obj)
        {
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:rrgjqqhjsj8";

            var customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = apiKey });
            var listRequest = customSearchService.Cse.List(QueryString);
            listRequest.Cx = searchEngineId;
            var search = listRequest.Execute();
            AllGoogleResult = new ObservableCollection<GoogleResult>(
                search.Items.Select(x => new GoogleResult(x))
                    .Where(
                        x => !string.IsNullOrEmpty(x.Id) && !string.IsNullOrEmpty(x.Title))
                    .OrderBy(x => x.Title).ToList());
        }
    }

    [ImplementPropertyChanged]
    public class GoogleResult
    {
        public GoogleResult(Result r)
        {
            Link = r.Link;

            Title = r.Title;
            Snippet = !string.IsNullOrEmpty(r.Snippet) ? r.Snippet.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Replace("  ", " ") : "No Overview";

            var myUri = new Uri(Link);
            var param2 = HttpUtility.ParseQueryString(myUri.Query).Get("id");
            var param1 = HttpUtility.ParseQueryString(myUri.Query).Get("seriesid");
            var param = param1;
            if (string.IsNullOrEmpty(param)) param = param2;
            Id = param;
            BrowseTo = new RelayCommand(DoBrowseTo, o => true);
        }

        public GoogleResult(Series sc)
        {
            if (sc == null) return;
            Link = $"http://thetvdb.com/?id={sc.Id}&tab=series";
            Title = $"{sc.Name} ({sc.FirstAired.Year.ToString("0000")})";
            if (!string.IsNullOrEmpty(sc.Overview))
                Snippet = sc.Overview.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Replace("  ", " ");
            else Snippet = "No Snippet";
            Id = GetTvDbId(sc).ToString();
            BrowseTo = new RelayCommand(DoBrowseTo, o => true);
        }

        public string Snippet { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        public string Link { get; set; }

        public RelayCommand BrowseTo { get; set; }

        private void DoBrowseTo(object obj)
        {
            Process.Start(Link);
        }

        private static int GetTvDbId(Series ret)
        {
            if (ret == null) return -1;
            var id = ret.SeriesId;
            if (id < 1) id = ret.Id;
            if (id < 1) id = -1;
            return id;
        }
    }

    public static class StringHelper
    {
        public static string ToCleanString(this string s)
        {
            var invalidPathChars = Path.GetInvalidFileNameChars();
            var cly = s.Replace(":", ",");
            cly = invalidPathChars.Aggregate(cly, (current, rr) => current.Replace(rr, ' '));
            var myTi = new CultureInfo("en-US", false).TextInfo;
            return Regex.Replace(cly, @"\s+", " ").TrimEnd().TrimStart();
        }
    }
}