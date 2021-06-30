using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using PropertyChanged;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace MovieNamer
{
    [ImplementPropertyChanged]

    public class TmDbViewModel
    {
        private TMDbClient _client = new TMDbClient("125e475654c781837d575da127e4d7bd");
        private const string MovieDataLibrary = @"\\admin-pc\g\MovieDataLibrary";
        private string _postersize;
        private string _backdropsize;
        private string _baseurl;
        private Movie _movie;
        public SearchMovie SelectedSearchMovie { get; set; }
        public ObservableCollection<SearchMovie> AllSearchResults { get; set; }
        public WebClient BackDropClient { get; set; }
        public string SearchMovieName { get; set; }
        public string SearchMovieYear { get; set; }

        public TmDbViewModel()
        {
            FetchConfig(_client);
            BackDropClient = new WebClient();
        }

        private async void FetchConfig(TMDbClient client)
        {
            System.IO.Directory.CreateDirectory(MovieDataLibrary);
            var configXml = new FileInfo(System.IO.Path.Combine(MovieDataLibrary, "config.xml"));


            if (configXml.Exists && configXml.LastWriteTimeUtc >= DateTime.UtcNow.AddHours(-1))
            {

                var xml = File.ReadAllText(configXml.FullName, Encoding.Unicode);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                client.SetConfig(Serializer.Deserialize<TMDbConfig>(xmlDoc));
            }
            else
            {
                Console.WriteLine("Getting new config");
                client.GetConfig();

                Console.WriteLine("Storing config");
                var xmlDoc = Serializer.Serialize(client.Config);
                File.WriteAllText(configXml.FullName, xmlDoc.OuterXml, Encoding.Unicode);
            }

            var u = await _client.GetMovieGenres();
            GenreDictionary = u.ToDictionary(x => x.Id, y => y.Name);
        }

        public Dictionary<int, string> GenreDictionary { get; set; }

        public string SelectedMovieGenreList
        {
            get
            {
                if (SelectedSearchMovie == null) return "";
                var r = SelectedSearchMovie.GenreIds.Select(x => GenreDictionary[x]).ToArray();
                return string.Join(", ", r);
            }
        }

        public async Task<ObservableCollection<SearchMovie>> GetMovies()
        {
            var n = SearchMovieName;
            var s = SearchMovieYear;
            var res =
                string.IsNullOrEmpty(s)
                    ? await _client.SearchMovie(n, 0, true, 0)
                    : await _client.SearchMovie(n, 0, true, int.Parse(s));
            AllSearchResults = new ObservableCollection<SearchMovie>(res.Results.ToList());
            SelectedSearchMovie = AllSearchResults.FirstOrDefault();
            return AllSearchResults;


        }

        public ImageSource Backdrop
        {
            get { return new BitmapImage(new Uri(BackdropUri, UriKind.RelativeOrAbsolute)); }
        }


        public string BackDropfilepath()
        {
            if (SelectedSearchMovie == null || SelectedSearchMovie.BackdropPath == null) return "pack://application:,,,/images/background.jpg";

            if (string.IsNullOrEmpty(_backdropsize))
            {
                _client.GetConfig();
                _backdropsize = "w1280";
                _baseurl = _client.Config.Images.BaseUrl;
                foreach (var size in _client.Config.Images.BackdropSizes)
                {
                    Console.WriteLine(size);
                    if (size.EndsWith("1280")) _backdropsize = size;
                }
            }

            var ret = string.Format("{0}{1}{2}", _baseurl, _backdropsize, SelectedSearchMovie.BackdropPath);
            return ret;
        }

        public Visibility HasData
        {
            get { return SelectedSearchMovie == null ? Visibility.Hidden : Visibility.Visible; }
        }

        public string BackdropUri
        {
            get
            {
                return BackDropfilepath();
            }
        }

        public Visibility PosterVisibility
        {
            get
            {
                return (!string.IsNullOrEmpty(PosterUri)) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string PosterUri
        {
            get
            {
                if (SelectedSearchMovie == null || SelectedSearchMovie.PosterPath == null) return "";
                if (!string.IsNullOrEmpty(_postersize))
                    return string.Format("{0}{1}{2}", _baseurl, _postersize, SelectedSearchMovie.PosterPath);
                _client.GetConfig();
                _postersize = "w500";
                _baseurl = _client.Config.Images.BaseUrl;
                foreach (var size in _client.Config.Images.PosterSizes)
                {
                    Console.WriteLine(size);
                    if (size.EndsWith("500")) _postersize = size;
                }
                return string.Format("{0}{1}{2}", _baseurl, _postersize, SelectedSearchMovie.PosterPath);
            }
        }

        public bool IsActive { get; set; }



        //public event PropertyChangedEventHandler PropertyChangedEvet;
        //public virtual void OnMyPropertyChanged(string propertyName)
        //{
        //    var propertyChanged = PropertyChangedEvet;
        //    if (propertyChanged != null)
        //    {
        //        propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        public async void GetMovieDetails()
        {
            if (SelectedSearchMovie == null) return;
            _movie = await _client.GetMovie(SelectedSearchMovie.Id, MovieMethods.Credits | MovieMethods.Keywords);
            AllCasts = _movie.Credits.Cast;
            AllCrews = _movie.Credits.Crew;
            Producer = AllCrews.FirstOrDefault(x => x.Job == "Producer");
            Director = AllCrews.FirstOrDefault(x => x.Job == "Director");
            SetTargetFilename();
        }

        public void SetTargetFilename()
        {
            if (_movie == null) return;
            if (_movie.ReleaseDate != null)
                SafeFileName =
                    string.Format("{0} ({1}){2}",
                        SelectedSearchMovie.Title.ToSafeFilename(),
                        _movie.ReleaseDate.Value.Year,
                        DropFileExtension);
            else
                SafeFileName = SelectedSearchMovie.Title.ToSafeFilename() + DropFileExtension.ToLower();
        }

        public void RenameFile()
        {
            var root = System.IO.Path.GetPathRoot(DropFileName);
            if (root == null) return;
            var newpath = Path.Combine(root, "_watch",
                "MyMovies." + SafeFileName.Substring(0, 1),
                Path.GetFileNameWithoutExtension(SafeFileName));
            Directory.CreateDirectory(newpath);

            var newname = Path.Combine(newpath, SafeFileName);
            File.Move(DropFileName, newname);
            DropFileName = newname;
        }

        public string DropFileName { get; set; }
        public string DropNameOnly { get; set; }
        public string SafeFileName { get; set; }

        public Crew Director { get; set; }

        public Crew Producer { get; set; }

        public List<Crew> AllCrews { get; set; }

        public List<Cast> AllCasts { get; set; }
        public string DropFileExtension { get; set; }
    }

    

    public static class Helper
    {
        public static string ToSafeFilename(this string s)
        {
            return s.Replace(":", ",")
                .Replace("/", " ")
                .Replace("\\", " ")
                .Replace("*", " ")
                .Replace("?", ".")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", " ")
                .Replace("  ", " ");
        }
    }
}
