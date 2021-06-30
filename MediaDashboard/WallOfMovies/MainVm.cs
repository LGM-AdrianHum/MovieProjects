using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using PropertyChanged;
using Relays;
using Transitionals.Controls;
using UserControls;
using UtilityFunctions;

namespace WallOfMovies
{
    [ImplementPropertyChanged]
    public class MainVm
    {
        public MainVm()
        {
            Refresh = new RelayCommand(DoRefresh, o => true);
            MyDispatcherTimer=new DispatcherTimer();
            MyDispatcherTimer.Interval=TimeSpan.FromSeconds(2);
            MyDispatcherTimer.Tick += MyDispatcherTimer_Tick;
        }

        private void MyDispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                MyDispatcherTimer.IsEnabled = false;
                var xc = EveryMovie.FirstOrDefault(x => !x.HasPic);
                if (xc == null) return;
                xc.SetPoster();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                MyDispatcherTimer.IsEnabled = true;
            }
        }

        public RelayCommand Refresh { get; set; }
        public DispatcherTimer MyDispatcherTimer { get; set; }
        public ObservableCollection<MoviePic> EveryMovie { get; set; }

        public void DoRefresh(object obj)
        {
            IsBusy = true;
            var bgw = new BackgroundWorker();
            bgw.DoWork += (ss, ee) =>
            {
                var qlib = new DirectoryInfo(@"Q:\MyMovies.Library");
                var everymovie =
                    qlib.GetFiles("*.*", SearchOption.AllDirectories)
                        .AsParallel()
                        .Where(x => x.Extension.IsMovie())
                        .Select(x => new MoviePic(x))
                        .OrderBy(x => x.Name)
                        .ThenBy(x => x.Size);
                ee.Result = everymovie;
            };
            bgw.RunWorkerCompleted += (ss, ee) =>
            {
                if (ee.Result != null)
                    EveryMovie = new ObservableCollection<MoviePic>((IEnumerable<MoviePic>)ee.Result);
                IsBusy = false;
                MyDispatcherTimer.IsEnabled = true;
            };
            bgw.RunWorkerAsync();
        }

        public bool IsBusy { get; set; }
    }
    [ImplementPropertyChanged]
    public class MoviePic
    {
        public MoviePic()
        {
        }

        public MoviePic(FileInfo x)
        {
            DirectoryPath = x.DirectoryName;
            NameOnly = Path.GetFileNameWithoutExtension(x.Name);
            Name = x.Name;
            Size = x.Length;
            if (DirectoryPath == null) return;
            PosterPath = Path.Combine(DirectoryPath, "Poster.jpg");
            if (!File.Exists(PosterPath))
            {
                PosterPath = "";
                HasPic = true;
            }
        }

        public object ContentControl { get; set; }

        public string SizeP => Size.ToBytes();

        public long Size { get; set; }

        public string Name { get; set; }

        public string NameOnly { get; set; }

        public string PosterPath { get; set; }
        public ImageSource Pic { get; set; }
        public string DirectoryPath { get; set; }
        public bool HasPic { get; set; }

        public void SetPoster()
        {
            try
            {
                var img = new Image
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    Source = ImageHelpers.CreateImageSource(PosterPath)
                };
                ContentControl = img;
                HasPic = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);                
                throw;
            }
        }
    }
}