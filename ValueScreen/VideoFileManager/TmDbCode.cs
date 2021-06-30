using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using PropertyChanged;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace VideoFileManager
{
    [ImplementPropertyChanged]
    public class TmDbCode
    {
        private Movie _movie;

        public TmDbCode()
        {
            GetDetailsFromTmDb = new RelayCommand(DoGetDetailsFromTmDb, o => true);
        }

        public string Overview { get; set; }

        public string DefaultBackdropUri { get; set; }

        public string DefaultPosterUri { get; set; }

        public List<ImageData> Posters { get; set; }
        public IList<string> PostersUri { get; set; }
        public List<ImageData> BackDrops { get; set; }
        public IList<string> BackDropsUri { get; set; }

        public RelayCommand GetDetailsFromTmDb { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }

        public string Line => $"[{Id}] {Name} ({Year})";

        public string FileTitle
        {
            get
            {
                var rr = System.IO.Path.GetInvalidPathChars();
                var ll = Name.Replace(":", ", ");
                ll = rr.Aggregate(ll, (current, r) => current.Replace(r, ' '));
                if (!string.IsNullOrEmpty(Year)) ll = ll + $" ({Year})";
                ll= Regex.Replace(ll, @"\s+", " ");
                return ll;
            }
        }



        private void DoGetDetailsFromTmDb(object obj)
        {
            var client = new TMDbClient("125e475654c781837d575da127e4d7bd");


            _movie = client.GetMovie(Id,
                MovieMethods.Images | MovieMethods.Credits);


            Console.WriteLine($"Movie title: {_movie.Title}");
            foreach (var cast in _movie.Credits.Cast)
                Console.WriteLine($"{cast.Name} - {cast.Character}");

            Overview = _movie.Overview;
            BackDrops = _movie.Images.Backdrops;
            Posters = _movie.Images.Posters;
            BackDropsUri = new List<string>();
            PostersUri = new List<string>();
            foreach (var imguri in _movie.Images.Backdrops)
                BackDropsUri.Add($"http://image.tmdb.org/t/p/w1280/{imguri.FilePath}");

            foreach (var imguri in _movie.Images.Posters)
                PostersUri.Add($"http://image.tmdb.org/t/p/w500/{imguri.FilePath}");
            {
                var imguri = _movie.Images.Posters.FirstOrDefault();
                if (imguri != null) Thumbnail = $"http://image.tmdb.org/t/p/w154/{imguri.FilePath}";
                DefaultPosterUri = PostersUri.FirstOrDefault();
                if (!string.IsNullOrEmpty(_movie.PosterPath)) DefaultPosterUri = $"http://image.tmdb.org/t/p/w500/{_movie.PosterPath}";

            }
            var dm = Application.Current.MainWindow.DataContext as DirectoryHelper;
            if (dm == null) return;
            dm.SelectedMovie = this;
            DefaultBackdropUri = BackDropsUri.FirstOrDefault();

            

        }

        public void SaveBackData()
        {
            if (_movie == null) return;
            var dta = new movienfo()
            {
                id = _movie.ImdbId,
                title = _movie.Title,
                originaltitle = _movie.OriginalTitle,
                sorttitle = _movie.Title,
                rating = _movie.Popularity.ToString(),
                year = _movie.ReleaseDate?.Year.ToString() ?? "",
                votes = _movie.VoteCount.ToString(),
                outline = _movie.Overview,
                plot = _movie.Overview,
                tagline = _movie.Tagline,
                runtime = _movie.Runtime?.ToString(),
                thumb = Thumbnail,
                mpaa = "Not Available",
                playcount = "0",
                filenameandpath = "..",
                genre = string.Join(", ", _movie.Genres.Select(x => x.Name).ToArray())

            };

            var dm = Application.Current.MainWindow.DataContext as DirectoryHelper;
            if (dm == null) return;

            var imgcache =
                System.IO.Path.Combine(@"k:\ImageCache\", 
                _movie.PosterPath.TrimStart('\\').TrimStart('/')
                );

            if (System.IO.File.Exists(imgcache))
                System.IO.File.Copy(imgcache,
                    System.IO.Path.Combine(dm.CurrentDirectory.Path, "poster" + System.IO.Path.GetExtension(imgcache))
                );

            Helper.SaveMovie(
                System.IO.Path.Combine(
                dm.CurrentDirectory.Path, "movie.nfo"), dta);

            dm.RefreshCurrentItems();

        }


        public string Thumbnail { get; set; }

        public bool HasData()
        {
            return _movie != null;
        }
    }
}
