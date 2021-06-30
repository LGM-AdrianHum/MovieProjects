using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PropertyChanged;
using Path = System.IO.Path;

namespace VideoFileManager
{
    /// <summary>
    /// Interaction logic for MovieVisualizer.xaml
    /// </summary>
    public partial class MovieVisualizer
    {
        private readonly MovieDetails _dm = new MovieDetails();
        public MovieVisualizer()
        {
            InitializeComponent();
            DataContext = _dm;
        }
    }

    [ImplementPropertyChanged]
    public class MovieDetails
    {
        public bool isActive { get; set; } = false;
        public FileInfo MovieFileInfo { get; set; }
        public DirectoryInfo MovieDirectoryInfo { get; set; }
        public string FullPath { get; set; }
        public string FullMovieName { get; set; }
        public string MovieName { get; set; }
        public string MovieYear { get; set; }
        public List<string> AllFiles { get; set; }

        public void SetMovieFileFromDirectoryName()
        {
            MovieName = MovieDirectoryInfo.Name;
            var newname = Path.Combine(MovieDirectoryInfo.FullName, MovieName);
            if (!File.Exists(newname)) MovieFileInfo.MoveTo(newname);
            MovieName = MovieFileInfo.Name;
        }

        public void SetMkvToolsProperty()
        {
            if (MovieFileInfo.Extension != ".mkv") return;
            var r = $"mkvpropedit ";

            var procinfo = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo()
                {
                    Arguments = $"movie.mkv --edit info --set \"title = {MovieFileInfo.Name}\"",
                    FileName = "mkvpropedit.exe",

                }
            };
            isActive = true;
            procinfo.Exited += Procinfo_Exited;
            procinfo.WaitForExit();
        }

        private void Procinfo_Exited(object sender, EventArgs e)
        {
            isActive = false;
        }

        public void SetMovieDetails(string myFullPath)
        {
            if (Directory.Exists(myFullPath)) MovieDirectoryInfo = new DirectoryInfo(myFullPath);
            if (!File.Exists(myFullPath)) return;
            MovieFileInfo = new FileInfo(myFullPath);
            MovieDirectoryInfo = MovieDirectoryInfo.Parent;
        }
    }
}
