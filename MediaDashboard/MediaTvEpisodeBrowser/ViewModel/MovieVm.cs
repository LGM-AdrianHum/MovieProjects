using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using PropertyChanged;
using Relays;
using TMDbLib.Client;
using UtilityFunctions;

namespace MediaTvEpisodeBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class MovieVm
    {
        private MovieItem _selectedMovieFiles;

        public MovieVm()
        {
            Refresh = new RelayCommand(DoRefresh, o => true);
            ExecuteQuery = new RelayCommand(DoExecuteQuery, o => true);
            MovieIt=new RelayCommand(DoMovieIt, o=>true);
        }

        private void DoMovieIt(object obj)
        {
            if (SelectedMovieFiles == null) return;
            if (GoogleSelected == null) return;
            SelectedMovieFiles.RenameAndMove(GoogleSelected);
            SelectedMovieFiles.BackColor=new SolidColorBrush(Colors.DarkOliveGreen);
        }

        public RelayCommand MovieIt { get; set; }

        private void DoExecuteQuery(object obj)
        {
            if (obj.ToString() == "tmdb")
            {
                if(QueryString==SelectedMovieFiles.GoogleSearch) QueryString = SelectedMovieFiles.QueryString;
                ExecuteSearchByTmdb();
                return;
            }

            //AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY
            //tvdb:003372514794118234263:rrgjqqhjsj8
            //tmdb:003372514794118234263:36td8wej70u
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:36td8wej70u";
            
            CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = customSearchService.Cse.List(QueryString);
            listRequest.Cx = searchEngineId;
            Search search = listRequest.Execute();
            AllGoogleResult=new ObservableCollection<GoogleResult>(
                search.Items.Select(x=>new GoogleResult(x))
                    .Where(x=> !string.IsNullOrEmpty(x.Id) && !string.IsNullOrEmpty(x.Title) && x.Link.Contains("/movie/"))
                    .OrderBy(x=>x.Title).ToList());
        }

        private void ExecuteSearchByTmdb()
        {
            var client = new TMDbClient("125e475654c781837d575da127e4d7bd");
            //var yr = 0;
            //int.TryParse(y, out yr);
            var l = client.SearchMovie(QueryString, 0, true) ?? client.SearchMovie(QueryString, 0, true, 0);
            if (l?.Results == null) return;
            AllGoogleResult = new ObservableCollection<GoogleResult>(l.Results.Select(x => new GoogleResult(x)));
        }

        public ObservableCollection<GoogleResult> AllGoogleResult { get; set; }
        public GoogleResult GoogleSelected { get; set; }

        public bool ModeSearch { get; set; }

        public RelayCommand ExecuteQuery { get; set; }

        private void DoRefresh(object obj)
        {
            var a1 = Properties.Settings.Default.MovieRaw.Split('|');
            var f1 = new List<FileInfo>();
            foreach (var a in a1)
            {
                var d = new DirectoryInfo(a);
                var b = d.EnumerateFiles("*.*", SearchOption.AllDirectories).AsParallel().Where(x=>x.Name.IsMovie()).Select(x => x);
                f1.AddRange(b);
            }
            var f2 = f1.AsParallel().Select(x => new MovieItem(x)).OrderBy(x => x.FinalName).ToList();
            AllMovieFiles = new ObservableCollection<MovieItem>(f2);
        }

        public ObservableCollection<MovieItem> AllMovieFiles { get; set; }
        public string QueryString { get; set; }
        public MovieItem SelectedMovieFiles
        {
            get { return _selectedMovieFiles; }
            set
            {
                _selectedMovieFiles = value;
                ModeSearch = true;
                QueryString = _selectedMovieFiles.GoogleSearch ;
            }
        }

        public bool IsOpen { get; set; }

        public RelayCommand Refresh { get; set; }
    }
}