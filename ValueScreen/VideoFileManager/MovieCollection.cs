using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PropertyChanged;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace VideoFileManager
{
    [ImplementPropertyChanged]
    public class MovieCollection
    {
        private Movie _movie;
        public bool IsActive { get; set; }
        public IList<string> AllRows { get; private set; }

        public RelayCommand DoFilter { get; set; }

        public MovieCollection()
        {
            DoFilter = new RelayCommand(ExecuteDoFilter, o => true);
        }

        private void ExecuteDoFilter(object obj)
        {
            Console.WriteLine("ExecFilter");
            ReadDirectoryTree();
        }

        public IList<FileInfo> AllFiles { get; set; }
        public void ReadDirectoryTree()
        {
            IsActive = true;

            var nn = new List<FileInfo>();
            var bgw = new BackgroundWorker();
            bgw.RunWorkerCompleted += (ss, ee) =>
            {
                IsActive = false;
                AllFiles = nn;
            };

            bgw.DoWork += (ss, ee) =>
            {
                _worklist = new Queue<string>();
                _worklist.Enqueue(@"k:\BtSync\Movie");
                while (_worklist.Count > 0)
                {
                    nn.AddRange(DoDirectory());
                }
            };

            bgw.RunWorkerAsync();
        }

        private Queue<string> _worklist;
        public List<FileInfo> DoDirectory()
        {
            var s = _worklist.Dequeue();
            var rd1 = Directory.GetFiles(s, "*.mkv");
            var rd2 = Directory.GetFiles(s, "*.avi");
            var rd3 = Directory.GetFiles(s, "*.mp4");
            var rd4 = Directory.GetFiles(s, "*.m4v");

            var u = new List<FileInfo>();
            u.AddRange(rd1.ToList().Select(x => new FileInfo(x)));
            u.AddRange(rd2.ToList().Select(x => new FileInfo(x)));
            u.AddRange(rd3.ToList().Select(x => new FileInfo(x)));
            u.AddRange(rd4.ToList().Select(x => new FileInfo(x)));
            foreach(var d in Directory.EnumerateDirectories(s))
            _worklist.Enqueue(d);

            return u;


        }


        public void ReadSource()
        {
            IsActive = true;
            var bg = new BackgroundWorker();
            bg.RunWorkerCompleted += (sender, args) => { IsActive = false; };
            bg.DoWork += (sender, args) =>
            {
                AllRows =
                    File.ReadAllLines(@"\\admin-pc.local\MyMovies.All\Scandata.txt")
                        .ToList();
            };
            bg.RunWorkerAsync();
        }

        public void ReadTmDb()
        {
            IsActive = true;
            var bg = new BackgroundWorker();
            bg.RunWorkerCompleted += (sender, args) => { IsActive = false; };
            bg.DoWork += (sender, args) =>
            {
                GetTmDb();
            };
            bg.RunWorkerAsync();
        }


        public void GetTmDb()
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

        }

        public string Thumbnail { get; set; }

        public string DefaultPosterUri { get; set; }

        public List<string> PostersUri { get; set; }

        public List<string> BackDropsUri { get; set; }

        public List<ImageData> Posters { get; set; }

        public List<ImageData> BackDrops { get; set; }

        public string Overview { get; set; }

        public int Id { get; set; }
    }
}
