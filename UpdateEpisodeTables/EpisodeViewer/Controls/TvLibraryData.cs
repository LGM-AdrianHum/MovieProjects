using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EpisodeViewer.Properties;
using EpisodeViewer.ViewModel;
using PropertyChanged;

namespace EpisodeViewer.Controls
{
    [ImplementPropertyChanged]
    public class VideoDirectoryData
    {
        private VideoItem _selectedVideoItem;

        public VideoDirectoryData()
        {
            RawLibrary = new DirectoryInfo(Settings.Default.RawDirectory);
            if (!RawLibrary.Exists) return;
            var rr =
                RawLibrary.GetFiles("*.*", SearchOption.AllDirectories)
                    .AsParallel()
                    .Where(x => x.Extension.IsMovie())
                    .Select(x => new VideoItem(x));
            AllVideoFiles = new ObservableCollection<VideoItem>(rr);
        }

        public DirectoryInfo RawLibrary { get; set; }

        public ObservableCollection<VideoItem> AllVideoFiles { get; set; }
        public event EventHandler SelectionMade;
        public VideoItem SelectedVideoItem
        {
            get { return _selectedVideoItem; }
            set
            {
                _selectedVideoItem = value;
                var handler = SelectionMade;
                handler?.Invoke(this, null);
            }
        }
    }

    [ImplementPropertyChanged]
    public class VideoItem
    {
        public VideoItem(FileInfo fileInfo)
        {
            VideoFileInfo = fileInfo;
            SearchKey = fileInfo.Name.MakeSearchKey();
        }

        public string SearchKey { get; set; }

        public FileInfo VideoFileInfo { get; set; }
        public string FullName => VideoFileInfo?.FullName;
        public string Name => VideoFileInfo?.Name;

        public string ParentName => VideoFileInfo?.Directory?.Name;
        public override string ToString()
        {
            return Name;
        }
    }


    [ImplementPropertyChanged]
    public class TvLibraryData
    {
        private TvLibraryItem _selectedDirectory;

        public TvLibraryData()
        {
            DirLibrary = new DirectoryInfo(Settings.Default.LibDirectory);
            if (!DirLibrary.Exists) return;
            var rr =
                DirLibrary.GetDirectories()
                    .SelectMany(x => x.GetDirectories())
                    .AsParallel()
                    .Select(x => new TvLibraryItem(x))
                    .OrderBy(x => x.Name)
                    .ToList();
            AllDirectories = new ObservableCollection<TvLibraryItem>(rr);
        }

        public DirectoryInfo DirLibrary { get; set; }


        public ObservableCollection<TvLibraryItem> AllDirectories { get; set; }

        public TvLibraryItem SelectedDirectory
        {
            get { return _selectedDirectory; }
            set
            {
                _selectedDirectory = value;
                var handler = SelectionMade;
                handler?.Invoke(this, null);
            }
        }

        public event EventHandler SelectionMade;


        [ImplementPropertyChanged]
        public class TvLibraryItem
        {
            public TvLibraryItem(DirectoryInfo d)
            {
                DirInfo = d;
                var rx = new Regex(@"(?<libdir>.*)\\(?<libnum>\d{1,4})\\(?<therest>.*)(\\(?<sub>.*))?");
                var mat = rx.Match(d.FullName);
                LibDir = mat.Groups["libdir"].Value;
                LibNum = mat.Groups["libnum"].Value;
                ShowName = mat.Groups["therest"].Value;
                SubDirectory = mat.Groups["sub"].Value;
                SearchKey = ShowName.MakeSearchKey();
                var df = d.GetFiles("tvdb_*.zip").FirstOrDefault();
                if (df != null)
                {
                    rx = new Regex(@"\d{1,99}");
                    mat = rx.Match(df.Name);
                    var tid = mat.Value;
                    int i;
                    if (int.TryParse(tid, out i)) TvDbId = i;
                }
            }

            public string SearchKey { get; set; }

            public int TvDbId { get; set; }

            public string SubDirectory { get; set; }

            public string ShowName { get; set; }

            public string LibNum { get; set; }

            public string LibDir { get; set; }

            public DirectoryInfo DirInfo { get; set; }
            public string FullName => DirInfo?.FullName;
            public string Name => DirInfo?.Name;
        }

        
    }

    public static class StringHelper
    {
        public static string MakeSearchKey(this string s)
        {
            var regex = new Regex("[^a-zA-Z0-9]");
            return regex.Replace(s, "");
        }
    }

}