using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media;
using PropertyChanged;
using Relays;
using TVDB.Model;

namespace EpisodeViewer.ViewModel
{
    [ImplementPropertyChanged]
    public class EpisodePresenter
    {
        private readonly Episode _ep;

        public EpisodePresenter(Episode episode, string showname)
        {
            _ep = episode;
            FindEpisode = new RelayCommand(DoFindEpisode, o => true);
            ShowName = showname;
        }

        public string ShowName { get; set; }

        private void DoFindEpisode(object obj)
        {
            var rgx = new Regex("[^a-zA-Z0-9 ]");
            var sname = rgx.Replace(ShowName, " ");
            var url = $"https://rarbg.to/torrents.php?search={sname}+{SeasonEpisode}";
            Process.Start(url);
        }

        public int SeasonNumber => _ep.SeasonNumber;
        public string SeasonEpisode => _ep.SeasonEpisode;
        public string Overview => _ep.Overview;
        public string FirstAired => _ep.FirstAired.ToString("dd-MMM-yyyy");
        public string Name => _ep.Name;
        public List<FilePresenter> AllFiles { get; set; }
        public int EpisodeNumber => _ep.Number;
        public bool HasEpisodes { get; set; }
        public SolidColorBrush HasEpisodesColor { get; set; }

        public RelayCommand FindEpisode { get; set; }
    }
}