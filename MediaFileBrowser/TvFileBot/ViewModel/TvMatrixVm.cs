using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using PropertyChanged;
using Relays;
using Transitionals;
using Transitionals.Controls;
using TvDbSearchControl;
using TvFileBot.Controls;
using TvFileBot.Properties;
using TVDB.Model;
using TVDB.Web;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class TvMatrixVm
    {
        private static readonly string[] ValidImageExtensions = {".png", ".jpg", ".jpeg", ".bmp", ".gif"};
        private static readonly string[] ValidVideoExtensions = ".avi|.iso|.m4v|.mkv|.mp4|.mpg|.vob".Split('|');
        private readonly List<ImageSource> _imageSources = new List<ImageSource>();
        private double _gridHeight;

        private double _gridWidth;
        private FileObject _selectedVideoFile;
        private int _selindex;

        public TvMatrixVm()
        {
            //MyGridUpdateDispatcher = new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};
            //MyGridUpdateDispatcher.Tick += MyGridUpdateDispatcher_Tick;
            Trans = new List<TransitionElement>();
            RefreshWorkingDirectory = new RelayCommand(DoRefreshWorkingDirectory, o => true);
            SelectFileObject = new RelayCommand(DoSelectFileObject, o => true);
            ToggleSearching = new RelayCommand(DoToggleSearching, o => true);
            AuxCommand = new RelayCommand(DoAuxCommand, o => true);
            //LoadImageFolder(@"d:\Wallpaper&Images.Raw.001\Oboi-raznoe 29.03\Wallpapers Mix №158");
            //MyGridUpdateDispatcher.Start();
        }

        public Episode SelectedEpisode { get; set; }

        public bool HasFolder { get; set; }

        public SeriesDetails FromShowDetails { get; set; }

        public ObservableCollection<Episode> AllEpisodes { get; set; }

        public RelayCommand AuxCommand { get; set; }

        public RelayCommand ToggleSearching { get; set; }

        public RelayCommand SelectFileObject { get; set; }

        public RelayCommand RefreshWorkingDirectory { get; set; }


        public List<TransitionElement> Trans { get; set; }
        public DispatcherTimer MyGridUpdateDispatcher { get; set; }

        public string WorkingFolder { get; set; }

        public ObservableCollection<FileObject> AllVideoFiles { get; set; }

        public FileObject SelectedVideoFile
        {
            get { return _selectedVideoFile; }
            set
            {
                _selectedVideoFile = value;
                if (_selectedVideoFile == null) return;
                _selindex = AllVideoFiles.IndexOf(_selectedVideoFile);
                if (AllEpisodes != null && AllEpisodes.Any() && !string.IsNullOrEmpty(SelectedVideoSeasonEpisode) &&
                    SelectedVideoSeasonEpisode.Length >= 6)
                {
                    var sv = SelectedVideoSeasonEpisode.Substring(0, 6).ToUpper();
                    SelectedEpisode = AllEpisodes.FirstOrDefault(x => x.SeasonEpisode == sv);
                }
                if (IsLocked) return;
                SeriesName = _selectedVideoFile.Showname;
                TvDbId = "";
                if (SearchClassDetails == null) SearchClassDetails = new SearchClass();
                SearchClassDetails.SearchString = SeriesName;
            }
        }

        public SearchClass SearchClassDetails { get; set; }
        public bool IsLocked { get; set; }

        public string SeriesName { get; set; }

        public string SelectedVideoFileName => SelectedVideoFile?.Name;
        public string SelectedVideoFileDirectory => SelectedVideoFile?.DirectoryName;

        public string SelectedVideoSeasonEpisode
        {
            get
            {
                if (SelectedVideoFile?.Episodes != null && SelectedVideoFile.Episodes.Any())
                    return $"{SelectedVideoFile?.SeasonCodes}";
                return "";
            }
        }

        public double GridWidth
        {
            get { return _gridWidth; }
            set
            {
                _gridWidth = value;
                GridColumns = Convert.ToInt32(Math.Floor(_gridWidth/120));
                if (GridColumns == 0) GridColumns = 1;
                StarColumns = string.Join(",", Enumerable.Range(0, GridColumns).ToArray());
                UpdateGrid();
            }
        }

        public double GridHeight
        {
            get { return _gridHeight; }
            set
            {
                _gridHeight = value;
                GridRows = Convert.ToInt32(Math.Floor(_gridHeight/110));
                if (GridRows == 0) GridRows = 1;
                StarRows = string.Join(",", Enumerable.Range(0, GridRows).ToArray());
                UpdateGrid();
            }
        }

        public int GridColumns { get; set; } = 2;
        public int GridRows { get; set; } = 2;
        public string GridSize => $"{GridWidth} x {GridHeight}";

        public string StarRows { get; set; } = "0,1";
        public string StarColumns { get; set; } = "0,1";
        public RandomTransitionSelector RandomTsx { get; set; }
        public Grid TvMatrixGrid { get; set; }

        public bool IsSearching { get; set; }

        public string TvDbId { get; set; }

        private void DoAuxCommand(object obj)
        {
            if (obj == null) return;
            if (obj.ToString() == "load" && !string.IsNullOrEmpty(TvDbId))
            {
                var i = 0;
                if (!int.TryParse(TvDbId, out i)) return;
                GetData(i);
                return;
            }
            if (obj.ToString() == "rename")
            {
                if (SelectedVideoFile == null || !File.Exists(SelectedVideoFile.Fullname)) return;
                var ext = Path.GetExtension(SelectedVideoFile.Fullname).ToLower();
                if (string.IsNullOrEmpty(ext)) return;
                var a =
                    $"{FromShowDetails?.Series?.Name.ToCleanString()} - {SelectedEpisode?.SeasonEpisode} - {SelectedEpisode?.Name.ToCleanString()}{ext}";
                var b = Path.Combine(
                    Path.GetPathRoot(SelectedVideoFile.Fullname),
                    "MyTv.Library.Raw",
                    FromShowDetails?.Series?.Name.ToCleanString(),
                    "S" + SelectedEpisode.SeasonNumber.ToString("00")
                    );
                if (!Directory.Exists(b)) Directory.CreateDirectory(b);
                var c = Path.Combine(b, a);
                if (!File.Exists(c)) File.Move(SelectedVideoFile.Fullname, c);
                LoadWorkingDirectory();
            }
        }

        public async void GetData(int i)
        {
            var cl = new WebInterface("", Settings.Default.WorkingDirectory);
            var mm = await cl.GetMirrors();
            if (mm != null && mm.Any())
            {
                var mmm = mm.FirstOrDefault();
                var cc = cl.SaveFullSeriesById(Settings.Default.WorkingDirectory, i, mmm);
                if (cc != null) FromShowDetails = cc;
                else
                {
                    return;
                }
                AllEpisodes = cc.Series.Episodes;
                if (!string.IsNullOrEmpty(SelectedVideoSeasonEpisode) && SelectedVideoSeasonEpisode.Length >= 6)
                {
                    var sv = SelectedVideoSeasonEpisode.Substring(0, 6).ToUpper();
                    SelectedEpisode = AllEpisodes.FirstOrDefault(x => x.SeasonEpisode == sv);
                }
                HasFolder = cc.Series?.Status.Substring(0, 1).ToLower() == "c";
            }
        }

        private void DoToggleSearching(object obj)
        {
            IsSearching = !IsSearching;
        }

        private void DoSelectFileObject(object obj)
        {
            MessageBox.Show("TVs");
        }

        private void DoRefreshWorkingDirectory(object obj)
        {
            WorkingFolder = @"q:\MyTv.Library\002\Neon Genesis Evangelion\";
            LoadWorkingDirectory();
        }

        public void LoadWorkingDirectory()
        {
            if (string.IsNullOrEmpty(WorkingFolder)) return;
            var sources =
                from file in new DirectoryInfo(WorkingFolder).GetFiles("*.*", SearchOption.AllDirectories).AsParallel()
                where ValidVideoExtensions.Contains(file.Extension, StringComparer.InvariantCultureIgnoreCase)
                select new FileObject(file.FullName);
            AllVideoFiles = new ObservableCollection<FileObject>(sources.OrderBy(x => x.Fullname));
            SelectedVideoFile = AllVideoFiles[_selindex];
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            if (AllVideoFiles == null || !AllVideoFiles.Any()) return;
            var rmax = GridColumns*GridRows;
            var i = 0;
            while (Trans.Count <= rmax) Trans.Add(new TransitionElement {TransitionSelector = RandomTsx});

            for (var r = 0; r < GridRows; r++)
                for (var c = 0; c < GridColumns; c++)
                {
                    Grid.SetColumn(Trans[i], c);
                    Grid.SetRow(Trans[i], r);
                    Trans[i].Content = i < AllVideoFiles.Count
                        ? new FileObjectUc {DataContext = AllVideoFiles[i]}
                        : null;
                    i++;
                }
        }


        private void MyGridUpdateDispatcher_Tick(object sender, EventArgs e)
        {
            var rmax = GridColumns*GridRows;
            var i = 0;
            while (Trans.Count <= rmax) Trans.Add(new TransitionElement {TransitionSelector = RandomTsx});
            if (TvMatrixGrid == null) return;
            for (var r = 0; r < GridRows; r++)
                for (var c = 0; c < GridColumns; c++)
                {
                    if (!TvMatrixGrid.Children.Contains(Trans[i])) TvMatrixGrid.Children.Add(Trans[i]);
                    Grid.SetColumn(Trans[i], c);
                    Grid.SetRow(Trans[i], r);
                    if (Trans[i].Content == null)
                        Trans[i].Content = new Image {Source = _imageSources[i], Stretch = Stretch.UniformToFill};
                    i++;
                }
        }

        private void LoadImageFolder(string folder)
        {
            var sw = Stopwatch.StartNew();
            if (!Path.IsPathRooted(folder))
                folder = Path.Combine(Environment.CurrentDirectory, folder);
            if (!Directory.Exists(folder))
            {
                return;
            }
            var r = new Random(DateTime.Now.Millisecond);
            var sources = from file in new DirectoryInfo(folder).GetFiles().AsParallel()
                where ValidImageExtensions.Contains(file.Extension, StringComparer.InvariantCultureIgnoreCase)
                orderby r.Next()
                select
                    ImageHelpers.CreateImageSource(file.FullName, true);
            _imageSources.Clear();
            _imageSources.AddRange(sources);
            sw.Stop();
            Console.WriteLine($"Total time to load {_imageSources.Count} images: {sw.ElapsedMilliseconds}ms");
        }
    }

    //AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY
    //tvdb:003372514794118234263:rrgjqqhjsj8
}