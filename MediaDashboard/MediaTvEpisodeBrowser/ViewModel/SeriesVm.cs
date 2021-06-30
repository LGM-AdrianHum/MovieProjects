using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows.Media;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using MediaTvEpisodeBrowser.Properties;
using PropertyChanged;
using Relays;
using UserControls;

namespace MediaTvEpisodeBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class SeriesVm
    {
        private TvEpisodeFolder _selectedDirectory;

        public SeriesVm()
        {
            Refresh = new RelayCommand(DoRefresh, o => !IsBusy);
            UpdateData = new RelayCommand(DoUpdateData, o => true);
            ExecuteQuery = new RelayCommand(DoExecuteQuery, o=>true);
        }

        private void DoExecuteQuery(object obj)
        {
            //AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY
            //tvdb:003372514794118234263:rrgjqqhjsj8
            //tmdb:003372514794118234263:36td8wej70u
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:36td8wej70u";
            const string query = "the chipmunk adventure";
            CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = customSearchService.Cse.List(query);
            listRequest.Cx = searchEngineId;
            Search search = listRequest.Execute();
            foreach (var item in search.Items)
            {

                Uri myUri = new Uri(item.Link);
                string param1 = HttpUtility.ParseQueryString(myUri.Query).Get("id");

                Console.WriteLine($"Title : {item.Title}\r\n{item.Link}\r\n{item.Snippet}\r\n{param1}\r\n");
            }
            Console.ReadLine();
        }

        public RelayCommand ExecuteQuery { get; set; }

        public string ProgressStatus { get; set; }

        public RelayCommand UpdateData { get; set; }

        public bool IsOpenDrawer { get; set; }
        public RelayCommand Refresh { get; set; }
        public bool IsBusy { get; set; }
        public ObservableCollection<TvEpisodeFolder> AllDirectories { get; set; }

        public TvEpisodeFolder SelectedDirectory
        {
            get { return _selectedDirectory; }
            set
            {
                _selectedDirectory = value;
                _selectedDirectory.SetData();
                IsOpenDrawer = true;
                BannerData = ImageHelpers.CreateImageSource(_selectedDirectory.BannerName);
            }
        }

        public ImageSource BannerData { get; set; }

        private void DoUpdateData(object obj)
        {
            if (SelectedDirectory == null) return;
            UpdateImageSets(SelectedDirectory);
        }

        public async void UpdateImageSets(TvEpisodeFolder sk)
        {
            try
            {
                var _di = sk.Di;
                var posterfile = Path.Combine(_di.FullName, "Poster.jpg");
                var wc = new WebClient();
                foreach (var ban in sk.TvSeriesDetails.Banners)
                {
                    ProgressStatus = $"{sk.TvSeriesDetails.Banners.IndexOf(ban)} {ban.BannerPath}";
                    var uri = new Uri($"http://thetvdb.com/banners/{ban.BannerPath}");
                    var fname = ban.BannerPath;

                    if (!string.IsNullOrEmpty(fname))
                    {
                        var actualfile = Path.Combine(_di.FullName, "extrafanart", fname);
                        var actualpath = Path.GetDirectoryName(actualfile);
                        if (!Directory.Exists(actualpath)) Directory.CreateDirectory(actualpath);
                        if (!File.Exists(actualfile))
                        {
                            await wc.DownloadFileTaskAsync(uri, actualfile);
                            if (!File.Exists(posterfile)) File.Copy(actualfile, posterfile);
                        }
                    }
                }
                var epcol = sk.TvSeriesDetails.Series.Episodes;
                foreach (var eppic in epcol)
                {
                    ProgressStatus = eppic.PictureFilename;
                    var uri = new Uri($"http://thetvdb.com/banners/{eppic.PictureFilename}");
                    var fname = eppic.PictureFilename;
                    if (!string.IsNullOrEmpty(fname))
                    {
                        var actualfile = Path.Combine(_di.FullName, "extrafanart", fname);
                        var actualpath = Path.GetDirectoryName(actualfile);
                        if (!Directory.Exists(actualpath)) Directory.CreateDirectory(actualpath);
                        if (!File.Exists(actualfile)) await wc.DownloadFileTaskAsync(uri, actualfile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void DoRefresh(object obj)
        {
            IsBusy = true;
            var bgw = new BackgroundWorker();
            bgw.DoWork += (ss, ee) =>
            {
                var df = new List<DirectoryInfo>();
                for (var i = 1; i < 10; i++)
                {
                    var q = Path.Combine(Settings.Default.TvDirectories, i.ToString("000"));
                    var dq = new DirectoryInfo(q);
                    if (!dq.Exists) continue;
                    df.AddRange(dq.EnumerateDirectories());
                }

                ee.Result = df.AsParallel().Select(x => new TvEpisodeFolder(x)).ToList();
            };
            bgw.RunWorkerCompleted += (ss, ee) =>
            {
                var b = ee.Result as List<TvEpisodeFolder>;

                AllDirectories = new ObservableCollection<TvEpisodeFolder>(b);
                IsBusy = false;
            };
            bgw.RunWorkerAsync();
        }
    }
}