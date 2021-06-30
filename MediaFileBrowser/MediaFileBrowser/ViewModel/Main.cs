using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataLayerTest;
using DataLayerTest.NfoGenerator;
using PropertyChanged;
using Relays;
using TMDbLib.Objects.Movies;
using Microsoft.VisualBasic.FileIO;

namespace MediaFileBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        private readonly FileSystemWatcher _fsw = new FileSystemWatcher();
        private string _selectedPath;
        private ItemBase _selectedShortCut;
        private ItemBase _selectedChildFolder;

        public MainViewModel()
        {
            RenameParentFolder = new RelayCommand(DoRenameParentFolder, o => true);
            DeleteFileOrFolder = new RelayCommand(DoDeleteFileOrFolder, o => SelectedChildFolder != null);
            MakeFolderFromFile = new RelayCommand(DoMakeFolderFromFile, o => SelectedChildFolder != null);
            AcceptSearch = new RelayCommand(DoAcceptSearch, o => true);
            MovieSearchNow = new RelayCommand(DoMovieSearchNow, o => IsSearchAvailable);
            ShortCuts = Directory.GetDirectories(@"Q:\MyMovies.Library").Select(x => new ItemBase(x)).ToList();
            ShortCuts.Add(new ItemBase(@"k:\BtSync.Archive\movie"));
            SelectedShortCut = ShortCuts.FirstOrDefault();
            SelectionMainChange = new RelayCommand(DoSelectionMainChange, o => true);
            SearchForMovieDetails = new RelayCommand(DoSearchForMovieDetails, o => true);
            UpdateDetails = new RelayCommand(DoUpdateDetails, o => true);
            RenameFolderOrFile = new RelayCommand(DoRenameFolderOrFile, o => SelectedChildFolder != null);
            _fsw.Changed += _fsw_Changed;
            _fsw.Created += _fsw_Changed;
            _fsw.Deleted += _fsw_Deleted;
            _fsw.EnableRaisingEvents = false;
            IsSearchAvailable = false;
        }

        private void DoRenameParentFolder(object obj)
        {
            var parent = Path.GetDirectoryName(SelectedPath);
            if (string.IsNullOrEmpty(parent)) return;
            var original = SelectedPath;
            if (obj.ToString().Equals(""))
            {
                var newpath = Path.Combine(parent, SelectedMovieFullName);
                if (newpath.Equals(original, StringComparison.InvariantCultureIgnoreCase)) return;
                if (Directory.Exists(newpath)) return;
                Directory.Move(original, newpath);
                SelectedPath = newpath;
            }
            else if (obj.ToString().Equals("collection", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(CollectionName?.Trim())) return;
                var parentpath = $"[{CollectionName.ToCleanString()}]";
                var suggesteddrive = 0;
                var bn = @"Q:\MyMovies.Library\S004";
                for (var i = 1; i < 5; i++)
                {
                    bn = @"Q:\MyMovies.Library\S" + i.ToString("000");
                    if (!Directory.Exists(Path.Combine(bn, parentpath))) continue;
                    suggesteddrive = i;
                    break;
                }
                if (suggesteddrive == 0) suggesteddrive = 4;
                bn = @"Q:\MyMovies.Library\S" + suggesteddrive.ToString("000");
                var bnn = Path.Combine(bn, parentpath, SelectedMovieFullName);

                if (!Directory.Exists(bn)) Directory.CreateDirectory(bn);
                MoveDirectoryWithUi(original, bnn);
            }
            else if (obj.ToString().Equals("letter", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(SelectedMovieFullName?.Trim())) return;
                var sd = 0;
                var firstletter = SelectedMovieFullName.Substring(0, 1).ToLower();

                if (string.CompareOrdinal(firstletter, "k") < 0) sd = 1;
                else if (string.CompareOrdinal(firstletter, "t") < 0) sd = 3;
                else sd = 4;

                var rx = new Regex("[A-Za-z]");
                var l = rx.Match(firstletter);
                if (!l.Success)
                {
                    firstletter = "#";
                    sd = 3;
                }
                firstletter = firstletter.ToUpper();
                if (SelectedMovieFullName.StartsWith("The ", StringComparison.CurrentCultureIgnoreCase))
                {
                    firstletter = "The";
                    sd = 4;
                }
                var qn = Path.Combine(@"Q:\MyMovies.Library", sd.ToString("000"), "MyMovies." + firstletter);

                if (!Directory.Exists(qn)) Directory.CreateDirectory(qn);

                var qnn = Path.Combine(qn, SelectedMovieFullName);
                if (SelectedPath.ToLower().Equals(qnn,StringComparison.CurrentCultureIgnoreCase)) return; // Already in the correct place.
                var suffix = 0;
                while (Directory.Exists(qnn))
                {
                    qnn = Path.Combine(qn, SelectedMovieFullName + $" [{suffix}]");
                    suffix++;
                }
                MoveDirectoryWithUi(original, qnn);

            }

            DoRefreshDirectory(SelectedPath);
        }

        private void MoveDirectoryWithUi(string original, string bnn)
        {
            try
            {
                FileSystem.MoveDirectory(original, bnn, UIOption.AllDialogs);
                DeleteEmptyDirectories(original);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }

        private void DeleteEmptyDirectories(string original)
        {
            var par = Path.GetDirectoryName(original);
            while (!string.IsNullOrEmpty(par))
            {
                if (Directory.Exists(par) && (Directory.GetDirectories(par).Any() || Directory.GetFiles(par).Any()))
                {
                    SelectedPath = par;
                    return;
                }
                if (Directory.Exists(par))
                {
                    Directory.Delete(par);
                    par = Path.GetDirectoryName(par);
                }
            }
        }

        public RelayCommand RenameParentFolder { get; set; }

        public RelayCommand RenameFolderOrFile { get; set; }

        private void DoRenameFolderOrFile(object obj)
        {
            if (SelectedChildFolder == null) return;
            var original = SelectedChildFolder.FullName;
            if (!File.Exists(original)) return;
            var newname = Path.Combine(SelectedPath, RenameName);
            if (string.Equals(newname, original, StringComparison.CurrentCultureIgnoreCase)) return;
            var prefix = "_";
            while (File.Exists(newname))
            {
                newname = Path.Combine(SelectedPath, prefix + RenameName);
                prefix += "_";
            }
            File.Move(original, newname);
            SelectedChildFolder.FullName = newname;
            DoRefreshDirectory(SelectedPath);
        }

        public RelayCommand DeleteFileOrFolder { get; set; }

        private void DoDeleteFileOrFolder(object obj)
        {
            if (SelectedChildFolder == null) return;
            if (!(File.Exists(SelectedChildFolder.FullName) || Directory.Exists(SelectedChildFolder.FullName))) return;
            if (SelectedChildFolder.IsMovie || Directory.Exists(SelectedChildFolder.FullName))
            {
                var r = MessageBox.Show(SelectedChildFolder.Name, "Delete?", MessageBoxButton.OKCancel,
                    MessageBoxImage.Question);

                if (r == MessageBoxResult.OK && SelectedChildFolder.IsMovie)
                    File.Delete(SelectedChildFolder.FullName);
                else
                    Directory.Delete(SelectedChildFolder.FullName, true);

                ChildFolders.Remove(SelectedChildFolder);
                return;
            }

            if (!File.Exists(SelectedChildFolder.FullName)) return;
            File.Delete(SelectedChildFolder.FullName);
            ChildFolders.Remove(SelectedChildFolder);
        }

        public RelayCommand MakeFolderFromFile { get; set; }

        private void DoMakeFolderFromFile(object obj)
        {
            var a = SelectedChildFolder.FullName;
            // ReSharper disable once AssignNullToNotNullAttribute
            var b = Path.Combine(SelectedPath, Path.GetFileNameWithoutExtension(a));
            if (string.IsNullOrEmpty(b) || Directory.Exists(b)) return;
            Directory.CreateDirectory(b);
            var c = Path.GetFileName(a);
            if (string.IsNullOrEmpty(c)) return;
            if (File.Exists(a)) File.Move(a, Path.Combine(b, c));
            if (Directory.Exists(a)) Directory.Move(a, Path.Combine(b, c));
            DoRefreshDirectory(SelectedPath);
        }

        private void DoAcceptSearch(object obj)
        {
            IsSearchAvailable = false;
            TmDbId = SelectedSearchValue.Id;
            DoUpdateDetails(null);
        }

        public RelayCommand AcceptSearch { get; set; }

        private void DoMovieSearchNow(object obj)
        {
            SearchResults = new ObservableCollection<ItemBase>();
            var l = Processors.DoSearchFull(SearchName, SearchYear);
            if (l == null || !l.Any()) l = Processors.DoSearchFull(SearchName, "");
            if (l == null || !l.Any())
            {
                SelectedSearchValue = null;
                SearchResults = null;
                return;
            }

            SearchResults = new ObservableCollection<ItemBase>(
                    l

                    .OrderByDescending(x => string.Equals(x.Title, SearchName, StringComparison.CurrentCultureIgnoreCase))
                    .ThenBy(x => x.Title)
                    .ThenByDescending(x => x.ReleaseDate?.Year.ToString() == SearchYear)
                    .ThenBy(x => x.ReleaseDate)
                    .Select(x => new ItemBase(
                        $"{x.Title} ({x.ReleaseDate?.Year})", x.Overview, x.Id)));
            SelectedSearchValue = SearchResults.FirstOrDefault();
        }

        public ItemBase SelectedSearchValue { get; set; }

        public ObservableCollection<ItemBase> SearchResults { get; set; }

        public RelayCommand MovieSearchNow { get; set; }

        public string CurrentLocation { get; set; }
        public List<ItemBase> ShortCuts { get; set; }

        public int ImageCountProgress { get; set; }

        public bool IsBusy { get; set; }

        public Movie MovieDataTmDb { get; set; }

        public string SearchYear { get; set; }

        public ParseValue GuessedParse { get; set; }

        public string SearchName { get; set; }


        public RelayCommand UpdateDetails { get; set; }

        public RelayCommand SearchForMovieDetails { get; set; }

        public string FullSelectedObject { get; set; }

        public RelayCommand SelectionMainChange { get; set; }

        public ItemBase SelectedShortCut
        {
            get { return _selectedShortCut; }
            set
            {
                _selectedShortCut = value;
                SelectedPath = _selectedShortCut.FullName;
                if (Directory.Exists(SelectedPath))
                {
                    DoRefreshDirectory(SelectedPath);
                }
            }
        }

        public string TagLine { get; set; }

        public string SelectedMovieYear { get; set; }

        public string SelectedMovieTitle { get; set; }

        public bool HasData { get; set; }

        public string Overview { get; set; }

        public int TmDbId { get; set; }

        public ImageSource PosterImageSource { get; set; }

        public ImageSource BackDropImage { get; set; }

        public ObservableCollection<ItemBase> ChildFolders { get; set; }

        public ItemBase SelectedChildFolder
        {
            get { return _selectedChildFolder; }
            set
            {
                _selectedChildFolder = value;
                if (value == null)
                {
                    RenameName = "";
                    RenameType = "Cannot Rename";
                    return;
                }
                if (File.Exists(value.FullName))
                {
                    var ext = Path.GetExtension(value.Name);
                    RenameName = SelectedMovieFullName + ext;
                    RenameType = "Rename File";
                }
                if (Directory.Exists(value.FullName))
                {
                    RenameName = SelectedMovieFullName;
                    RenameType = "Rename Directory";
                }
            }
        }

        public string RenameType { get; set; }

        public string RenameName { get; set; }

        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (Directory.Exists(value)) _selectedPath = value;
            }
        }

        private void DoUpdateDetails(object obj)
        {
            try
            {
                if (TmDbId != 0) GetMovieDetails();
                var fn = Path.GetFileName(SelectedPath);
                var gp = MovieNameHelper.MovieParse(fn) ??
                         MovieNameHelper.MovieParse2(fn) ?? MovieNameHelper.MovieParse3(fn);
                string sn;
                string sy;
                if (gp != null)
                {
                    sn = gp.Movie;
                    sy = gp.Year;
                }
                else
                {
                    sn = Path.GetFileNameWithoutExtension(fn);
                    sy = "";
                }
                SearchName = sn;
                SearchYear = sy;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void GetMovieDetails()
        {
            IsBusy = true;
            ImageCountProgress = 0;
            var bgw = new BackgroundWorker { WorkerReportsProgress = true };
            bgw.DoWork += (ss, ee) =>
            {
                bgw.ReportProgress(0, $"Retrieving TmDb data...");
                MovieDataTmDb = Processors.GetMovie(TmDbId);
                var db = new MovieData();
                var mvdata = db.TmDbMovies.FirstOrDefault(x => x.TmDbId == TmDbId);
                var mnfo = new movie();
                var mvinfo = Path.Combine(SelectedPath, "movie.nfo");
                bgw.ReportProgress(0, $"Updating movie.nfo...");
                mnfo.LoadFrom(mvdata);
                mnfo.Save(mvinfo);
                if (MovieDataTmDb != null) BackgroundUpdateImages(bgw);
            };
            bgw.RunWorkerCompleted += (ss, ee) =>
            {
                IsBusy = false;
                DoRefreshDirectory(SelectedPath);
            };
            bgw.ProgressChanged += (ss, ee) =>
            {
                ImageCountProgress = ee.ProgressPercentage;
                ProgressMessage = ee.UserState?.ToString();
            };
            bgw.RunWorkerAsync();
        }

        public string ProgressMessage { get; set; }

        private void BackgroundUpdateImages(BackgroundWorker bgw)
        {
            bgw.ReportProgress(0,"Processing Images");
            var db = new MovieData();
            Processors.ProcessImages(MovieDataTmDb);
            var wc = new WebClient();
            var bthumbs = Path.Combine(SelectedPath, "extrathumbs");
            if (!Directory.Exists(bthumbs)) Directory.CreateDirectory(bthumbs);
            var bfanart = Path.Combine(SelectedPath, "extrafanart");
            if (!Directory.Exists(bfanart)) Directory.CreateDirectory(bfanart);
            var icount = 0;
            if (!string.IsNullOrEmpty(MovieDataTmDb.BackdropPath))
            {
                var bb = MovieDataTmDb.BackdropPath?.TrimStart('/');
                if (!string.IsNullOrEmpty(bb))
                {
                    var rurl = $"http://image.tmdb.org/t/p/w1280/{bb}";
                    var ff = Path.Combine(bfanart, bb);
                    wc.DownloadFile(new Uri(rurl), ff);
                    var f2 = Path.Combine(SelectedPath, "backdrop.jpg");
                    if (File.Exists(f2)) File.Delete(f2);
                    if (File.Exists(ff)) File.Copy(ff, f2);
                }
            }

            if (!string.IsNullOrEmpty(MovieDataTmDb.PosterPath))
            {
                var bb = MovieDataTmDb.PosterPath?.TrimStart('/');
                if (!string.IsNullOrEmpty(bb))
                {
                    var rurl = $"http://image.tmdb.org/t/p/w500/{bb}";
                    var ff = Path.Combine(bfanart, bb);
                    wc.DownloadFile(new Uri(rurl), ff);
                    var f2 = Path.Combine(SelectedPath, "poster.jpg");
                    if (File.Exists(f2)) File.Delete(f2);
                    if (File.Exists(ff)) File.Copy(ff, f2);
                }
            }


            foreach (var qp in db.TmdbResourceFilenames.Where(x => x.BackdropFilenameId == TmDbId))
            {
                icount++;
                var bb = qp.FilePath?.TrimStart('/');
                bgw.ReportProgress(icount, $"Processing Backdrops\r\n{bb}");
                if (string.IsNullOrEmpty(bb)) continue;
                var rurl = "";
                var ff = "";
                if (icount < 5)
                {
                    rurl = $"http://image.tmdb.org/t/p/w780/{bb}";
                    ff = Path.Combine(bthumbs, "thumb" + icount.ToString("000") + ".jpg");
                    wc.DownloadFile(new Uri(rurl), ff);
                }
                rurl = $"http://image.tmdb.org/t/p/w1280/{bb}";
                ff = Path.Combine(bfanart, bb);
                wc.DownloadFile(new Uri(rurl), ff);
            }
            foreach (var qp in db.TmdbResourceFilenames.Where(x => x.BackdropFilenameId == TmDbId))
            {
                icount++;
                var bb = qp.FilePath?.TrimStart('/');
                bgw.ReportProgress(icount, $"Processing Posters\r\n{bb}");
                if (string.IsNullOrEmpty(bb)) continue;
                var rurl = $"http://image.tmdb.org/t/p/w500/{bb}";
                var ff = Path.Combine(bfanart, bb);
                wc.DownloadFile(new Uri(rurl), ff);
            }
        }

        private void DoSearchForMovieDetails(object obj)
        {
            IsSearchAvailable = !IsSearchAvailable;
            var a = Application.Current.MainWindow;
            var myTextBlock = (TextBox)a.FindName("SearchTextBox");
            myTextBlock?.Focus();
        }

        public bool IsSearchAvailable { get; set; }

        private static void _fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            //DoRefreshDirectory(SelectedPath);
        }

        private void _fsw_Changed(object sender, FileSystemEventArgs e)
        {
            //DoRefreshDirectory(SelectedPath);
        }

        private void DoSelectionMainChange(object obj)
        {
            SearchName = "";
            SearchYear = "";
            SearchResults = null;
            SelectedSearchValue = null;
            TmDbId = 0;
            var o = obj as ItemBase;
            if (o == null) return;
            FullSelectedObject = o.FullName;
            if (Directory.Exists(SelectedPath)) SelectedPath = o.FullName;

            DoRefreshDirectory(SelectedPath);

            if (File.Exists(FullSelectedObject)) SelectedChildFolder = ChildFolders.FirstOrDefault(x => x.FullName == FullSelectedObject);
        }

        private void DoRefreshDirectory(object obj)
        {
            HasData = false;
            //IsBusy = true;
            var flag = false;
            ImageSource bg = null;
            ImageSource ps = null;
            _fsw.EnableRaisingEvents = false;
            var rObj = obj.ToString();

            if (!Directory.Exists(rObj)) return;

            var par = Directory.GetParent(rObj).FullName;
            var l = new List<ItemBase> { new ItemBase(par, "parent") };
            l.AddRange(Directory.GetDirectories(rObj).Select(x => new ItemBase(x, "folder")));
            l.AddRange(Directory.GetFiles(rObj).Select(x => new ItemBase(x)));

            var bkdrp = Path.Combine(rObj, "backdrop.jpg");
            if (File.Exists(bkdrp))
            {
                var uriSource = new Uri(bkdrp, UriKind.Absolute);
                var imgTemp = new BitmapImage();
                imgTemp.BeginInit();
                imgTemp.CacheOption = BitmapCacheOption.OnLoad;
                imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                imgTemp.UriSource = uriSource;
                imgTemp.EndInit();
                bg = imgTemp;
            }

            var bkposter = Path.Combine(rObj, "poster.jpg");
            if (File.Exists(bkposter))
            {
                var uriSource = new Uri(bkposter, UriKind.Absolute);
                var imgTemp = new BitmapImage();
                imgTemp.BeginInit();
                imgTemp.CacheOption = BitmapCacheOption.OnLoad;
                imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                imgTemp.UriSource = uriSource;
                imgTemp.EndInit();
                ps = imgTemp;
                flag = true;
            }


            IsBusy = false;
            ChildFolders?.Clear();
            ChildFolders = new ObservableCollection<ItemBase>(l.ToList());

            UpdateFromNfo();
            PosterImageSource = ps;
            BackDropImage = bg;
            HasData = flag;

            _fsw.Path = rObj;
            _fsw.EnableRaisingEvents = true;

        }

        private void UpdateFromNfo()
        {
            var mvinfo = Path.Combine(SelectedPath, "movie.nfo");
            if (File.Exists(mvinfo))
                LoadNfo(mvinfo);
            else
            {
                var dd = Directory.GetFiles(SelectedPath, "*.tmdb");
                if (!dd.Any()) return;
                var dq = Path.GetFileNameWithoutExtension(dd.FirstOrDefault());
                int dqi;
                if (!int.TryParse(dq, out dqi)) return;
                TmDbId = dqi;
                if (TmDbId > 0) GetMovieDetails();
                if (!File.Exists(mvinfo)) return;
                LoadNfo(mvinfo);
                HasData = true;
            }
        }

        private void LoadNfo(string mvinfo)
        {
            var mvdata = movie.LoadFile(mvinfo);
            TmDbId = mvdata.tmdbid;
            Overview = mvdata.outline;
            SelectedMovieTitle = mvdata.title;
            SelectedMovieYear = mvdata.year;
            CollectionName = mvdata.set;
            SelectedMovieFullName = mvdata.title.ToCleanString() +
                                    ((string.IsNullOrEmpty(SelectedMovieYear) || SelectedMovieYear == "0")
                                        ? ""
                                        : $" ({SelectedMovieYear})");
            FolderLetter = mvdata.title.Substring(0, 1).ToUpper();
            var rx = new Regex("[A-Za-z]");
            var l = rx.Match(FolderLetter);
            if (!l.Success) FolderLetter = "#";
            if (mvdata.title.StartsWith("The ", StringComparison.CurrentCultureIgnoreCase))
                FolderLetter = "The";
            TagLine = mvdata.tagline;
            HasData = true;
        }

        public string SelectedMovieFullName { get; set; }

        public string FolderLetter { get; set; }

        public string CollectionName { get; set; }


        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

    }


}