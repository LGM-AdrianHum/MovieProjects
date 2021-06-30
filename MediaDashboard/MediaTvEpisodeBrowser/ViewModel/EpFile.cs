using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using PropertyChanged;
using TVDB.Model;
using UtilityFunctions.Movie;

namespace MediaTvEpisodeBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class EpFile
    {
        public EpFile(Episode p, string seriesName)
        {
            Episode = p;
            BackColor = new SolidColorBrush(Colors.Gray);
            SeriesName = seriesName;
            BackupName = "empty";
            
        }

        public string SeriesName { get; set; }

        public EpFile()
        {
        }

        public string EpisodePicture
        {
            get
            {
                if (FileInfo == null) return null;
                if (string.IsNullOrEmpty(Episode?.PictureFilename)) return null;
                var name = Path.Combine(
                    FileInfo.FullName,
                    "extrafanart",
                    Episode.PictureFilename.Replace("/","\\"));
                return File.Exists(name) ? name : null;
            }
        }

        public string ProperName=>  $"{SeriesName.ToCleanString()} - {SeasonEpisode} - {(string.IsNullOrEmpty(Name) ? string.Empty : Name.ToCleanString())}";
        public Episode Episode { get; set; }
        public FileInfo FileInfo { get; set; }
        public int SeasonNumber => Episode?.SeasonNumber ?? -1;
        public SolidColorBrush BackColor { get; set; }
        public string BackupName { get; set; }
        public string Name => string.IsNullOrEmpty(Episode?.Name) ? BackupName : Episode.Name;
        public string SeasonEpisode => string.IsNullOrEmpty(Episode?.SeasonEpisode) ? "S??E??" : Episode.SeasonEpisode;
        public bool IsMissing { get; set; }
        public bool IsDuplicate{ get; set; }
        public override string ToString()
        {
            return $"{SeriesName} - {SeasonEpisode} - {Name}";
        }
    }
}