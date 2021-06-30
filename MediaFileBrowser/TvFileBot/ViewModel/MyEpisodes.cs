using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Media;
using PropertyChanged;
using Relays;
using TvFileBot.Controls;
using TVDB.Model;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class MyEpisodes
    {
        private Episode _episode;

        public MyEpisodes(Episode x)
        {
            _episode = x;

            SeasonEpisode = x.SeasonEpisode;
            EpisodeBanner = x.PictureFilename;
            Name = x.Name;
            Overview = x.Overview;
            Backcolor = new SolidColorBrush(Colors.LimeGreen);
            var fa = x.FirstAired;
            if (fa.Year > 1900)
            {
                FirstAired = fa.ToString("dd-MMM-yyyy");
                if (fa > DateTime.Now) Backcolor = new SolidColorBrush(Colors.DarkRed);
            }
            RenameFile = new RelayCommand(DoRenameFile, o => true);
            OpenFolder = new RelayCommand(DoOpenFolder, o => true);
        }


        public void LoadImage(string wd)
        {
            if (EpisodeBannerData != null) return;
            if (string.IsNullOrEmpty(EpisodeBanner)) return;
            var justfile = Path.GetFileName(EpisodeBanner);
            ValidateFolder(wd);
            var posterpath = Path.Combine(wd, "extrafanart", justfile);
            if (!File.Exists(posterpath)) DownloadEpisodeArt(wd);
            EpisodeBannerData = ImageHelpers.CreateImageSource(posterpath, true);
        }

        public ImageSource EpisodeBannerData { get; set; }

        private void DownloadEpisodeArt(string wd)
        {
            var uri = new Uri($"http://thetvdb.com/banners/{EpisodeBanner}");
            var wc = new WebClient();
            var posterpath = Path.Combine(wd, "extrafanart", Path.GetFileName(EpisodeBanner));
            wc.DownloadFileCompleted += (ss, ee) =>
            {
                EpisodeBannerData = ImageHelpers.CreateImageSource(posterpath, true);
            };
            wc.DownloadFileAsync(uri, posterpath);
        }

        public void ValidateFolder(string wd)
        {
            var posterpath = Path.Combine(wd, "extrafanart");
            if (Directory.Exists(posterpath)) return;
            Directory.CreateDirectory(posterpath);
        }

        public bool HasFile { get; set; }
        public bool NeedsRename { get; set; }
        public int Count { get; set; }
        public RelayCommand OpenFolder { get; set; }
        public RelayCommand RenameFile { get; set; }
        public SolidColorBrush Backcolor { get; set; }
        public string DirectoryName { get; set; }
        public string EpisodeBanner { get; set; }
        public string FileName { get; set; }
        public string FirstAired { get; set; }
        public string FullFilename { get; set; }
        public string Name { get; set; }
        public string NewFileName { get; set; }
        public string Overview { get; set; }
        public string SeasonEpisode { get; set; }
        public string ShowName { get; set; }

        private void DoOpenFolder(object obj)
        {
            if (Count == 0)
            {
                var s = $"{ShowName.Replace(" ", "+")}+{SeasonEpisode}+ettv";
                var r = $"https://rarbg.to/torrents.php?search={s}";
                Process.Start(r);
            }
            if (string.IsNullOrEmpty(DirectoryName)) return;

            Process.Start(DirectoryName);
        }

        private void DoRenameFile(object obj)
        {
            var newname = Path.Combine(DirectoryName, NewFileName);
            NeedsRename = false;
            if (File.Exists(newname) && Name.ToLower() == NewFileName.ToLower())
            {
                newname = Path.Combine(DirectoryName, "_" + NewFileName);
                File.Move(FullFilename, newname);
                newname = Path.Combine(DirectoryName, NewFileName);
            }

            File.Move(FullFilename, newname);
            FileName = NewFileName;
            FullFilename = newname;
        }
    }
}