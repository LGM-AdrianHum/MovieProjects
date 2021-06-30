using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Data;
using System.Windows.Media;
using PropertyChanged;
using Relays;
using TvDbSearchControl;
using TvFileBot.Controls;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class DirectoryManagerVm
    {
        private ShowDirectory _selectedDirectoryShow;
        private string _selectedSeason;
        private MyEpisodes _selectedEpisode;

        public DirectoryManagerVm()
        {
            RefreshWorkingDirectory = new RelayCommand(DoRefreshWorkingDirectory, o => true);
            
            IsDrawerOpen = true;
        }

        public bool IsDrawerOpen { get; set; }

        public RelayCommand RefreshWorkingDirectory { get; set; }

        public ShowDirectory SelectedDirectoryShow
        {
            get { return _selectedDirectoryShow; }
            set
            {
                BannerData = null;
                PosterData = null;
                _selectedDirectoryShow = value;
                if (_selectedDirectoryShow == null) return;

                if (!_selectedDirectoryShow.Load())
                {
                    SearchClassDetails = new SearchClass {SearchString = _selectedDirectoryShow.Name};
                    IsSearching = true;
                    return;
                }

                ShowStatus = _selectedDirectoryShow.SeriesInfo.Series.Status;
                var ban = Path.Combine(_selectedDirectoryShow.FullName, "Banner.jpg");
                var pos = Path.Combine(_selectedDirectoryShow.FullName, "Poster.jpg");
                RemoveZeroByteFile(ban);
                RemoveZeroByteFile(pos);
                if (File.Exists(ban)) BannerData = ImageHelpers.CreateImageSource(ban, true);
                if (File.Exists(pos)) PosterData = ImageHelpers.CreateImageSource(pos, true);
                AllFiles = Directory.GetFiles(_selectedDirectoryShow.FullName, "*.*", SearchOption.AllDirectories).ToList();
                AllEpisodes =
                    new ObservableCollection<MyEpisodes>(
                        _selectedDirectoryShow?.SeriesInfo?.Series?.Episodes.Select(x => new MyEpisodes(x)));
                AllEpisodesView = CollectionViewSource.GetDefaultView(AllEpisodes);
                SelectedEpisode = AllEpisodes.FirstOrDefault();
                Seasons =
                    new ObservableCollection<string>(
                        _selectedDirectoryShow?.SeriesInfo?.Series?.Episodes.Select(x => "S"+x.SeasonNumber.ToString("00")).Distinct());
                Seasons.Insert(0,"All");
                SelectedSeason = Seasons.FirstOrDefault();

                AllEpisodesView.Filter = ImplementFilter;
                foreach (var v in AllEpisodes)
                {
                    var fif = AllFiles.FirstOrDefault(x => x.ToLower().Contains(v.SeasonEpisode.ToLower()));
                    v.ShowName = _selectedDirectoryShow.SeriesInfo.Series.Name;
                    if (fif == null) continue;
                    v.Count= AllFiles.Count(x => x.ToLower().Contains(v.SeasonEpisode.ToLower()));
                    v.FullFilename = fif;
                    v.FileName = Path.GetFileName(fif);
                    v.DirectoryName = Path.GetDirectoryName(fif);
                    
                    var ext = Path.GetExtension(fif);
                    v.NewFileName =
                        $"{_selectedDirectoryShow.SeriesInfo.Series.Name.ToCleanString()} - {v.SeasonEpisode} - {v.Name.ToCleanString()}{ext}";
                    v.NeedsRename = v.FileName != v.NewFileName;
                    v.Backcolor=new SolidColorBrush(Colors.DarkGreen);
                }
                if (string.IsNullOrEmpty(_selectedDirectoryShow.SeriesInfo.Series.Banner))
                {
                    var theBanner = _selectedDirectoryShow.SeriesInfo.Banners.FirstOrDefault();
                    if (theBanner != null)
                    {
                        _selectedDirectoryShow.SeriesInfo.Series.Banner = theBanner.BannerPath;
                        _selectedDirectoryShow.Save(_selectedDirectoryShow.TvDbId.ToString());
                    }
                }

                if (!File.Exists(ban) && !string.IsNullOrEmpty(_selectedDirectoryShow.FullBannerPath))
                {
                    IsBannerLoading = true;
                    var wc = new WebClient();
                    wc.DownloadFileCompleted += WcBannerPathComplete;
                    wc.DownloadFileAsync(new Uri(_selectedDirectoryShow.FullBannerPath), ban);
                }

                if (!File.Exists(pos) && !string.IsNullOrEmpty(_selectedDirectoryShow.PosterPath))
                {
                    IsPosterLoading = true;
                    var wc = new WebClient();
                    wc.DownloadFileCompleted += WcPosterDownloadComplete;
                    wc.DownloadFileAsync(new Uri(_selectedDirectoryShow.FullPosterPath), pos);
                }


                if (_selectedDirectoryShow.TvDbId > 0) return;
                if (SearchClassDetails == null) SearchClassDetails = new SearchClass();
                SearchClassDetails.SearchString = _selectedDirectoryShow.Name;
                IsSearching = true;

                AllEpisodesView.Refresh();
                MoveToFirstEpisode();
            }
        }

        private bool ImplementFilter(object obj)
        {
            Console.Write("F");
            if (SelectedSeason == "All" || SelectedSeason=="") return true;
            var s = SelectedSeason.Substring(1);
            var i = 0;
            int.TryParse(s, out i);
            var it = obj as MyEpisodes;
            if (it != null)
                return it.SeasonEpisode.StartsWith(SelectedSeason,StringComparison.InvariantCultureIgnoreCase);
            else
                return true;
        }

        public string SelectedSeason
        {
            get { return _selectedSeason; }
            set
            {

                _selectedSeason = value;
                Console.WriteLine(value);
                AllEpisodesView.Refresh();
                MoveToFirstEpisode();
            }
        }

        private void MoveToFirstEpisode()
        {
            var enumerator = AllEpisodesView.GetEnumerator();
            enumerator.MoveNext(); // sets it to the first element
            SelectedEpisode = enumerator.Current as MyEpisodes;
        }

        public ICollectionView AllEpisodesView { get; set; }

        public ObservableCollection<string> Seasons { get; set; }

        public bool IsBannerLoading { get; set; }
        public bool IsPosterLoading { get; set; }
        public bool IsSearching { get; set; }
        public ImageSource BannerData { get; set; }
        public ImageSource PosterData { get; set; }
        public List<string> AllFiles { get; set; }
        public ObservableCollection<MyEpisodes> AllEpisodes { get; set; }
        public ObservableCollection<ShowDirectory> AllShows { get; set; }
        
        public SearchClass SearchClassDetails { get; set; }
        public string ShowStatus { get; set; }
        public string TvDbId { get; set; }

        public MyEpisodes SelectedEpisode
        {
            get { return _selectedEpisode; }
            set
            {
                _selectedEpisode = value;
                if (_selectedEpisode == null) return;
                _selectedEpisode.LoadImage(_selectedDirectoryShow.FullName);
            }
        }

        public bool IsDetailsDrawerOpen { get; set; }

        private void DoRefreshWorkingDirectory(object obj)
        {
            IsDrawerOpen = true;
            RefreshDirectory();
        }

        public void RefreshDirectory()
        {
            var ll = new List<ShowDirectory>();
            for (var i = 1; i < 10; i++)
            {
                var dinum = i.ToString("000");
                var dd = Path.Combine(@"Q:\MyTv.Library", dinum);
                if (!Directory.Exists(dd)) continue;
                var k = Directory.GetDirectories(dd).Select(x => new ShowDirectory(dinum, x));
                ll.AddRange(k);
            }
            AllShows = new ObservableCollection<ShowDirectory>(ll.OrderBy(x => x.Name));
            SelectedDirectoryShow = AllShows.FirstOrDefault();
        }

        private static void RemoveZeroByteFile(string ban)
        {
            var banInfo = new FileInfo(ban);
            if (banInfo.Exists && banInfo.Length == 0) banInfo.Delete();
        }

        private void WcPosterDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            IsPosterLoading = false;
            var pd = Path.Combine(_selectedDirectoryShow.FullName, "Poster.jpg");
            if (!File.Exists(pd)) return;
            IsPosterLoading = false;
            PosterData = ImageHelpers.CreateImageSource(pd, true);
        }

        private void WcBannerPathComplete(object sender, AsyncCompletedEventArgs e)
        {
            IsBannerLoading = false;
            var pd = Path.Combine(_selectedDirectoryShow.FullName, "Banner.jpg");
            if (!File.Exists(pd)) return;
            BannerData = ImageHelpers.CreateImageSource(pd, true);
        }

        
    }
}