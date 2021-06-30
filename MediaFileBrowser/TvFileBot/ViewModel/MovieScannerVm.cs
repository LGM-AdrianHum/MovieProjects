using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using PropertyChanged;
using Relays;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TvFileBot.Properties;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class MovieScannerVm
    {
        //private Dictionary<int, Movie> _searchcache=new Dictionary<int, Movie>();

        public MovieScannerVm()
        {
            RefreshWorkingDirectory = new RelayCommand(DoRefreshWorkingDirectory, o => true);
            FirstItemGoogle = new RelayCommand(DoFirstItemGoogle, o => true);
            GetTmdbData = new RelayCommand(DoGetTmdbData, o => true);
            SaveAndPack = new RelayCommand(DoSaveAndPack, o => true);
        }

        public string PackingDirectory { get; set; }

        public string PackingFilename { get; set; }

        public RelayCommand SaveAndPack { get; set; }

        public Movie SelectedMovieData { get; set; }

        public RelayCommand GetTmdbData { get; set; }

        public RelayCommand FirstItemGoogle { get; set; }

        public RelayCommand RefreshWorkingDirectory { get; set; }

        public ICollectionView AllMoviesView { get; set; }

        public ObservableCollection<MovieItem> AllMovies { get; set; }
        public MovieItem SelectedMovieItem { get; set; }

        private void DoSaveAndPack(object obj)
        {
            PackingFilename = SelectedMovieData.Title.ToCleanString();
            PackingDirectory = Path.Combine(

                // ReSharper disable once AssignNullToNotNullAttribute
                Path.GetPathRoot(SelectedMovieItem.Fullname),
                "MyMovies.Library.Temp",
                PackingFilename);

            if (!Directory.Exists(PackingDirectory)) Directory.CreateDirectory(PackingDirectory);
            var nfo = new NfoMovie();
            nfo.LoadFrom(SelectedMovieData);
            var nfofile = Path.Combine(PackingDirectory, "movie.nfo");
            nfo.Save(nfofile);
        }

        private void DoGetTmdbData(object obj)
        {
            //if (_searchcache == null) _searchcache = new Dictionary<int, Movie>();

            //var tmdbid = SelectedMovieItem?.GoogleSearchResult?.TmDbId;

            //if (!tmdbid.HasValue) return;
            //if (!_searchcache.ContainsKey(tmdbid.Value))
            //{
            //    var client = new TMDbClient("125e475654c781837d575da127e4d7bd");
            //    var res = client.GetMovie(tmdbid.Value,
            //        MovieMethods.AlternativeTitles |
            //        MovieMethods.Changes |
            //        MovieMethods.Credits |
            //        MovieMethods.Images |
            //        MovieMethods.Keywords |
            //        MovieMethods.Releases |
            //        MovieMethods.Similar |
            //        MovieMethods.Lists |
            //        MovieMethods.Reviews |
            //        MovieMethods.Undefined
            //        );
            //    if (res != null) _searchcache.Add(res.Id, res);
            //}
            //SelectedMovieData = _searchcache[tmdbid.Value];
        }

        private void DoFirstItemGoogle(object obj)
        {
            if (AllMovies == null) DoRefreshWorkingDirectory(obj);
            if (AllMovies == null) return;
            var j = AllMovies.FirstOrDefault(x => !x.ExecSearch);
            j?.DoGoogleQuery();
        }

        public void DoRefreshWorkingDirectory(object obj)
        {
            var folder = Settings.Default.RawMovieDirectory;
            var sources =
                new DirectoryInfo(folder).GetFiles("*.*", SearchOption.AllDirectories)
                    .AsParallel()
                    .Where(x => x.Extension.IsMovie())
                    .Select(x => new MovieItem(x))
                    .OrderBy(x => x.Filename)
                    .ToList();

            AllMovies = new ObservableCollection<MovieItem>(sources);
            SelectedMovieItem = AllMovies.FirstOrDefault();
            AllMoviesView = CollectionViewSource.GetDefaultView(AllMovies);
        }
    }
}