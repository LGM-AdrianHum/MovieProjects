using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using MediaDashboard.Properties;
using PropertyChanged;
using Relays;
using TVDB.Model;
using TVDB.Web;
using UserControls;
using UtilityFunctions.FolderHelper;
using UtilityFunctions.Movie;

namespace MediaDashboard.ViewModel
{
    [ImplementPropertyChanged]
    public class BaseDashboardVm
    {
        public enum SearchModeType
        {
            Tv,
            Movie
        }

        private int _episodeSel;
        private int _seasonSel;
        private GoogleResultItem _selectedGoogleResult;
        private FilterFiles.SmallInfo _selectedTvFile;
        private Dictionary<int, SeriesDetails> _tvcache = new Dictionary<int, SeriesDetails>();

        public BaseDashboardVm()
        {
            RefreshDashboard = new RelayCommand(DoRefreshCounts, o => true);
            GoogleSearch = new RelayCommand(DoGoogleSearch, o => true);
            SelectSearchItem = new RelayCommand(DoSelectSearchItem, o => true);
            LoadTvDetails = new RelayCommand(DoLoadTvDetails, o => TvDbId > 0);
            ToggleWindow = new RelayCommand(DoToggleWindow, o => true);
            ExecuteRename = new RelayCommand(DoExecuteRename, o => true);
        }


        public RelayCommand ExecuteRename { get; set; }

        public RelayCommand ToggleWindow { get; set; }

        public RelayCommand LoadTvDetails { get; set; }

        public RelayCommand SelectSearchItem { get; set; }

        public GoogleResultItem SelectedGoogleResult
        {
            get { return _selectedGoogleResult; }
            set
            {
                _selectedGoogleResult = value;
                if (value == null)
                {
                    SearchVisible = true;
                    return;
                }
                SearchVisible = false;
                TvDbId = (int) value?.TvdbId;
                TmDbId = (int) value?.TmdbId;
            }
        }

        public ImageSource BannerData { get; set; }

        public int SeasonSel
        {
            get { return _seasonSel; }
            set
            {
                _seasonSel = value;
                SetEpisodeMinMax();
            }
        }

        public int EpisodeSel
        {
            get { return _episodeSel; }
            set
            {
                _episodeSel = value;
                SetEpisodeName();
            }
        }

        public string EpisodeAirDate { get; set; }

        public string EpisodeName { get; set; }

        public int EpisodeMax { get; set; }

        public int EpisodeMin { get; set; }

        public int SeasonMax { get; set; }

        public int SeasonMin { get; set; }

        public SeriesDetails TvSeriesDetails { get; set; }

        public bool SearchVisible { get; set; }

        public int TmDbId { get; set; }

        public ObservableCollection<GoogleResultItem> AllGoogleResults { get; set; }


        public int TvDbId { get; set; }

        public RelayCommand GoogleSearch { get; set; }

        public FilterFiles.SmallInfo SelectedTvFile
        {
            get { return _selectedTvFile; }
            set
            {
                QueryString = "";
                SearchVisible = false;
                AllGoogleResults = null;
                _selectedTvFile = value;
                var pr = _selectedTvFile.Name.TvFileTryParse();
                if (string.IsNullOrEmpty(pr.Title))
                {
                    var rr = Path.GetDirectoryName(_selectedTvFile.FullName);
                    rr = Path.GetFileName(rr) + " " + _selectedTvFile.Name;
                    pr = rr.TvFileTryParse();
                }
                QueryString = pr.Title;
                FileEpisode = pr.EpisodeNumber;
                FileEpisodes = pr.Episodes;
                FileSeason = pr.SeasonInt;
                if (FileSeason <= SeasonMax) SeasonSel = FileSeason;
                if (FileEpisode <= EpisodeMax) EpisodeSel = FileEpisode;

                SetEpisodeName();
                SearchMode = SearchModeType.Tv;
            }
        }

        public List<int> FileEpisodes { get; set; }

        public int FileSeason { get; set; }

        public int FileEpisode { get; set; }

        public SearchModeType SearchMode { get; set; }
        public string QueryString { get; set; }


        public int SelectedTab { get; set; }

        public RelayCommand RefreshDashboard { get; set; }

        public ObservableCollection<FilterFiles.SmallInfo> AllMovie { get; set; }

        public ObservableCollection<FilterFiles.SmallInfo> AllTv { get; set; }

        public int IncomingTvCount { get; set; }

        public int IncomingMovieCount { get; set; }

        public int TvCount { get; set; }

        public int MovieCount { get; set; }

        private void DoToggleWindow(object obj)
        {
            var s = obj.ToString();
            switch (s)
            {
                case "googlesearch":
                {
                    SearchVisible = !SearchVisible;
                    break;
                }
            }
        }

        private void DoSelectSearchItem(object obj)
        {
            var r = obj as GoogleResultItem;
            MessageBox.Show(r?.Title);
        }

        private void DoGoogleSearch(object obj)
        {
            //tvdb:003372514794118234263:rrgjqqhjsj8
            //tmdb:003372514794118234263:36td8wej70u
            SearchVisible = true;
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            var searchEngineId = "003372514794118234263:rrgjqqhjsj8";
            if (SearchMode == SearchModeType.Movie) searchEngineId = "003372514794118234263:36td8wej70u";

            AllGoogleResults = new ObservableCollection<GoogleResultItem>();
            var customSearchService = new CustomsearchService(new BaseClientService.Initializer {ApiKey = apiKey});
            var listRequest = customSearchService.Cse.List(QueryString);
            listRequest.Cx = searchEngineId;
            var search = listRequest.Execute();
            if (search == null) return;
            var ress = search.Items.AsParallel().Select(x => new GoogleResultItem(x));
            AllGoogleResults = new ObservableCollection<GoogleResultItem>(ress.ToList());
            SelectedGoogleResult = null; //ress.FirstOrDefault();

        }

        private void DoExecuteRename(object obj)
        {
            var tempwd = Settings.Default.TvTemp;
            if (!Directory.Exists(tempwd)) Directory.CreateDirectory(tempwd);
            var v = Path.Combine(tempwd, TvSeriesDetails.Series.Name.ToCleanString(),
                $"S{SeasonSel.ToString("00")}");
            if (!Directory.Exists(v)) Directory.CreateDirectory(v);
            var v2 =
                $"{TvSeriesDetails.Series.Name.ToCleanString()} - S{SeasonSel.ToString("00")}E{EpisodeSel.ToString("00")} - {EpisodeName.ToCleanString()}{_selectedTvFile.Extension}";
            var v3 = Path.Combine(v, v2);
            if (!File.Exists(v3)) File.Move(_selectedTvFile.FullName, v3);
            DoRefreshCounts(null);
        }

        public async void RefreshBannerImage()
        {
            var tempwd = Settings.Default.TvTemp;
            if (!Directory.Exists(tempwd)) Directory.CreateDirectory(tempwd);
            var wc = new WebClient();
            var k = TvSeriesDetails.Series.Banner;
            if (string.IsNullOrEmpty(k)) return;
            var u = new Uri($"http://thetvdb.com/banners/{k}");
            var v = Path.Combine(tempwd, TvSeriesDetails.Series.Name.ToCleanString(),
                "banner.jpg");
            var v1 = Path.GetDirectoryName(v);
            if (!Directory.Exists(v1)) if (v1 != null) Directory.CreateDirectory(v1);

            if (!File.Exists(v)) await wc.DownloadFileTaskAsync(u, v);

            BannerData = ImageHelpers.CreateImageSource(v);
        }

        public void DoLoadTvDetails(object o)
        {
            if (TvDbId == 0) return;
            var pth = Path.Combine(Settings.Default.cache, "Tv", $"tvdb_{TvDbId}.zip");
            if (File.Exists(pth))
            {
                TvSeriesDetails = WebInterface.GetFullSeriesByFile(pth);
                RefreshBannerImage();
                SetSeasonMinMax();
            }
            else
            {
                DoGet(pth);
            }
        }

        private void SetSeasonMinMax()
        {
            if (TvSeriesDetails == null)
            {
                SeasonMin = 0;
                SeasonMax = 0;
                return;
            }

            SeasonMin = TvSeriesDetails.Series.Episodes.Min(x => x.SeasonNumber);
            SeasonMax = TvSeriesDetails.Series.Episodes.Max(x => x.SeasonNumber);
            SeasonSel = FileSeason;
            SetEpisodeMinMax();
            SetEpisodeName();
        }

        private void SetEpisodeMinMax()
        {
            if (TvSeriesDetails == null)
            {
                EpisodeMin = 0;
                EpisodeMax = 0;
                EpisodeSel = 0;
                return;
            }
            EpisodeMin = TvSeriesDetails.Series.Episodes.Where(x => x.SeasonNumber == SeasonSel).Min(x => x.Number);
            EpisodeMax = TvSeriesDetails.Series.Episodes.Where(x => x.SeasonNumber == SeasonSel).Max(x => x.Number);
            EpisodeSel = FileEpisode;
            SetEpisodeName();
        }

        private void SetEpisodeName()
        {
            EpisodeName = "";
            if (TvSeriesDetails == null) return;

            //var names=TvSeriesDetails.Series.Episodes.Select()

            //EpisodeName = xx?.Name;
            //EpisodeAirDate = xx?.FirstAired.ToString("dd-MMM-yyyy") + "@" + TvSeriesDetails.Series.AirsTime;
        }

        private async void DoGet(string pth)
        {
            var a = new WebInterface("");
            var ms = await a.GetMirrors();
            if (ms != null)
            {
                var m = ms.FirstOrDefault();
                TvSeriesDetails = a.SaveFullSeriesById(pth, TvDbId, m);
                SetSeasonMinMax();
                RefreshBannerImage();
            }
        }

        public void DoRefreshCounts(object o)
        {
            var k = Settings.Default.movie;
            AllMovie = Directory.Exists(k)
                ? new ObservableCollection<FilterFiles.SmallInfo>(FilterFiles.GetVideoFile(k).AsSmallInfos())
                : null;
            MovieCount = AllMovie?.Count ?? 0;
            TvCount = AllTv?.Count ?? 0;
            k = k.Replace(".Archive", "");
            IncomingMovieCount = Directory.Exists(k) ? FilterFiles.GetVideoFile(k).Count : 0;
            k = Settings.Default.tv;
            AllTv = Directory.Exists(k)
                ? new ObservableCollection<FilterFiles.SmallInfo>(FilterFiles.GetVideoFile(k).AsSmallInfos())
                : null;
            TvCount = AllTv?.Count ?? 0;
            k = k.Replace(".Archive", "");
            IncomingTvCount = Directory.Exists(k) ? FilterFiles.GetVideoFile(k).Count : 0;
        }
    }
}