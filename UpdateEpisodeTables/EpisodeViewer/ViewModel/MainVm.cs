using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PropertyChanged;
using Relays;
using TVDB.Model;
using TVDB.Web;

namespace EpisodeViewer.ViewModel
{
    [ImplementPropertyChanged]
    public class MainVm
    {
        private string _queryString;
        private int _seasonfilter;
        private MyDirInfo _selectedDirectory;
        private string _selectedSeason;

        public MainVm()
        {
            LeftIsOpen = false;
            MissingData = new RelayCommand(DoMissingData, o => true);
            Refresh = new RelayCommand(DoRefresh, o => true);
            SearchIsVisible = false;
            SearchVector = new GoogleSearcherVm();
            TriggerWindow = new RelayCommand(DoTriggerWindow, o => true);
            OpenCurrentDirectory = new RelayCommand(DoOpenCurrentDirectory, o => true);
            RebuildDirectoryInfo = new RelayCommand(DoRebuildDirectoryInfo, o => true);
        }

        public RelayCommand RebuildDirectoryInfo { get; set; }

        public RelayCommand OpenCurrentDirectory { get; set; }

        public GoogleSearcherVm SearchVector { get; set; }

        public RelayCommand TriggerWindow { get; set; }

        public bool LeftIsOpen { get; set; }

        public RelayCommand MissingData { get; set; }

        public ICollectionView EveryShowView { get; set; }

        public ObservableCollection<MenuItem> NoDataMenus { get; set; }

        public ObservableCollection<MyDirInfo> EveryShow { get; set; }

        public MyDirInfo SelectedDirectory
        {
            get { return _selectedDirectory; }
            set { UpdateSelectedDirectoryDetails(value); }
        }

        public ICollectionView AllFilesView { get; set; }

        public ObservableCollection<FilePresenter> AllFiles { get; set; }


        public string SelectedSeason
        {
            get { return _selectedSeason; }
            set
            {
                _selectedSeason = value;
                if (value == "All")
                {
                    AllEpisodesView.Filter = null;
                    AllEpisodesView.Refresh();
                }
                else
                {
                    var ik = value.Substring(1);
                    int i;
                    if (int.TryParse(ik, out i))
                    {
                        _seasonfilter = i;
                        AllEpisodesView.Filter = o =>
                        {
                            var item = o as EpisodePresenter;
                            return item?.SeasonNumber == _seasonfilter;
                        };
                    }
                }
                AllEpisodesView.Refresh();
            }
        }

        public ICollectionView AllEpisodesView { get; set; }

        public ObservableCollection<string> SeasonNumbers { get; set; }

        public bool NextAiredVisible { get; set; }

        public ObservableCollection<EpisodePresenter> AllEpisodes { get; set; }

        public string NextAiredNum { get; set; }

        public string NextAiredMonth { get; set; }

        public string NextAiredDay { get; set; }

        public string Status { get; set; }

        public Episode NextAired { get; set; }

        public SeriesDetails TvDbData { get; set; }

        public ImageSource PosterData { get; set; }

        public ObservableCollection<MyDirInfo> NoData { get; set; }

        public RelayCommand Refresh { get; set; }

        public string QueryString
        {
            get { return _queryString; }
            set
            {
                _queryString = value;
                if (_queryString.Length > 3)
                {
                    EveryShowView.Filter = o =>
                    {
                        var item = o as DirectoryInfo;
                        return item != null && item.Name.ToLower().Contains(_queryString.ToLower());
                    };
                }
                else
                {
                    EveryShowView.Filter = null;
                }
                EveryShowView.Refresh();
            }
        }

        public bool SearchIsVisible { get; set; }

        private static int GetTvDbId(SeriesDetails ret)
        {
            if (ret?.Series == null) return -1;
            var id = ret.Series.SeriesId;
            if (id < 1) id = ret.Id;
            if (id < 1) id = ret.Series.Id;
            if (id < 1) id = -1;
            return id;
        }

        private async void DoRebuildDirectoryInfo(object obj)
        {
            try
            {
                SetId(GetTvDbId(TvDbData).ToString());
                UpdateSelectedDirectoryDetails(SelectedDirectory);
                var wc = new WebClient();
                var k = TvDbData.Series.Banner;
                if (string.IsNullOrEmpty(k)) return;
                var u = new Uri($"http://thetvdb.com/banners/{k}");
                var v = Path.Combine(SelectedDirectory.Di.FullName, "banner.jpg");
                var v1 = Path.GetDirectoryName(v);
                if (!Directory.Exists(v1)) if (v1 != null) Directory.CreateDirectory(v1);
                if (!File.Exists(v)) wc.DownloadFile(u, v);
                k = TvDbData.Series.Poster;
                u = new Uri($"http://thetvdb.com/banners/{k}");
                v = Path.Combine(SelectedDirectory.Di.FullName, "Poster.jpg");
                if (!File.Exists(v)) await wc.DownloadFileTaskAsync(u, v);
                k = TvDbData.Banners.FirstOrDefault(x => x.Type == BannerTyp.fanart)?.BannerPath;
                u = new Uri($"http://thetvdb.com/banners/{k}");
                if (!string.IsNullOrEmpty(k))
                {
                    v = Path.Combine(SelectedDirectory.Di.FullName, "Backdrop.jpg");
                    if (!File.Exists(v)) await wc.DownloadFileTaskAsync(u, v);
                }

                foreach (var vq in TvDbData.Banners)
                {
                    var savepath = Path.Combine(SelectedDirectory.Di.FullName, "extrafanart",
                        vq.BannerPath.Replace("/", "\\").TrimStart('\\'));
                    var pth = Path.GetDirectoryName(savepath);
                    if (pth != null && !Directory.Exists(pth)) Directory.CreateDirectory(pth);
                    u = new Uri($"http://thetvdb.com/banners/{vq.BannerPath}");
                    await wc.DownloadFileTaskAsync(u, savepath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            SystemSounds.Beep.Play();
        }

        private void DoOpenCurrentDirectory(object obj)
        {
            if (SelectedDirectory == null) return;
            Process.Start(SelectedDirectory.Di.FullName);
        }

        private void DoTriggerWindow(object obj)
        {
            LeftIsOpen = !LeftIsOpen;
        }

        private void UpdateSelectedDirectoryDetails(MyDirInfo value)
        {
            _selectedDirectory = value;
            var pfile = _selectedDirectory.Di.GetFiles("Poster.jpg").FirstOrDefault();
            if (pfile != null)
            {
                PosterData = ImageHelpers.CreateImageSource(pfile.FullName);
            }
            var tvdata = _selectedDirectory.Di.GetFiles("tvdb_*.zip").FirstOrDefault();

            NextAired = null;
            NextAiredDay = "";
            NextAiredMonth = "";
            NextAiredNum = "";
            NextAiredVisible = false;

            if (tvdata != null)
            {
                TvDbData = WebInterface.GetFullSeriesByFile(tvdata.FullName);
                NextAired = TvDbData.Series.Episodes.FirstOrDefault(x => x.FirstAired >= DateTime.Today.AddDays(-3));
                if (NextAired != null)
                {
                    NextAiredDay = NextAired.FirstAired.ToString("ddd");
                    NextAiredMonth = NextAired.FirstAired.ToString("MMM");
                    NextAiredNum = NextAired.FirstAired.Day.ToString();
                    NextAiredVisible = true;
                }

                Status = TvDbData.Series.Status;
                SeasonNumbers =
                    new ObservableCollection<string>(
                        TvDbData.Series.Episodes.Select(x => $"S{x.SeasonNumber.ToString("00")}").Distinct());
                if (SeasonNumbers.Count > 1) SeasonNumbers.Insert(0, "All");
                AllEpisodes =
                    new ObservableCollection<EpisodePresenter>(
                        TvDbData.Series.Episodes.Select(x => new EpisodePresenter(x, TvDbData.Series.Name)));
                var ll = _selectedDirectory.Di.GetFiles("*.*", SearchOption.AllDirectories).AsParallel()
                    .Where(x => x.Extension.IsMovie())
                    .Select(x => new FilePresenter(x, TvDbData.Series.Name))
                    .ToList();
                foreach (var ve in AllEpisodes)
                {
                    ve.AllFiles = ll.Where(x =>
                        x.Name.ToLower().Contains("s" + ve.SeasonNumber.ToString("00"))
                        && (x.Name.ToLower().Contains("e" + ve.EpisodeNumber.ToString("00"))
                            || x.Name.ToLower().Contains("&" + ve.EpisodeNumber.ToString("00")))
                        ).ToList();

                    foreach (var vq in ve.AllFiles)
                    {
                        vq.Add(ve);
                    }

                    ve.HasEpisodes = ve.AllFiles.Any();

                    ve.HasEpisodesColor = ve.HasEpisodes
                        ? new SolidColorBrush(Colors.LightGreen)
                        : new SolidColorBrush(Colors.Red);
                }

                AllFiles = new ObservableCollection<FilePresenter>(ll);
                AllFilesView = CollectionViewSource.GetDefaultView(AllFiles);
                AllEpisodesView = CollectionViewSource.GetDefaultView(AllEpisodes);
                AllEpisodesView.SortDescriptions.Add(new SortDescription("SeasonEpisode",
                    ListSortDirection.Descending));
                AllEpisodesView.Refresh();
            }
        }

        private void DoMissingData(object obj)
        {
            MessageBox.Show(obj.GetType().ToString());
            var o = obj as MyDirInfo;

            if (o == null) return;
            SelectedDirectory = o;
            SearchIsVisible = true;
            SearchVector.SetParent(this);
            SearchVector.QueryString = SelectedDirectory.Name;
        }

        private void DoRefresh(object obj)
        {
            var everyshow = new List<MyDirInfo>();
            for (var i = 1; i < 10; i++)
            {
                var k = Path.Combine(@"Q:\MyTv.Library", i.ToString("000"));
                if (!Directory.Exists(k)) continue;
                var di = new DirectoryInfo(k);
                everyshow.AddRange(di.GetDirectories().Select(x => new MyDirInfo(x)));
            }
            var nodata = everyshow.Where(k => !k.Di.GetFiles("tvdb_*.zip").Any()).ToList();
            NoData = new ObservableCollection<MyDirInfo>(nodata.OrderBy(x => x.Name));
            EveryShow = new ObservableCollection<MyDirInfo>(everyshow.OrderBy(x => x.Name));
            EveryShowView = CollectionViewSource.GetDefaultView(EveryShow);
            EveryShowView.Refresh();
            NoDataMenus =
                new ObservableCollection<MenuItem>(
                    nodata.OrderBy(x => x.Name).Select(x => new MenuItem {Header = x.Name, CommandParameter = x}));
        }

        public async void SetId(string selectedId)
        {
            var wd = SelectedDirectory.Di.FullName;
            var client = new WebInterface(null, wd);
            var mm = await client.GetMirrors();
            var m = mm.FirstOrDefault();
            if (m == null) return;
            var idfile = SelectedDirectory.Di.GetFiles("tvdb_*.zip");
            while (idfile.Any())
            {
                var firstOrDefault = idfile.FirstOrDefault();
                firstOrDefault?.Delete();
                idfile = SelectedDirectory.Di.GetFiles("tvdb_*.zip");
            }

            client.SaveFullSeriesById(wd, int.Parse(selectedId), m);
            UpdateSelectedDirectoryDetails(SelectedDirectory);
            SearchIsVisible = false;
        }
    }
}