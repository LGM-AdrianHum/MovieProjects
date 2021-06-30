using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using PropertyChanged;
using Relays;
using tvd.Properties;
using TVDB.Model;
using TVDB.Utility;
using TVDB.Web;

namespace tvd.ViewModel
{
    [ImplementPropertyChanged]
    public class ViewModel
    {
        private int _episode1Input;
        private int _episode2Input;
        private int _episode3Input;
        private int _episode4Input;
        private int _seasonInput;
        private RawFileData _selectedFile;
        private Episode _selectedEpisode;

        public ViewModel()
        {
            MakeDirectoryTree();

            var r = new DirectoryInfo(Settings.Default.RawPath);
            if (r.Exists)
            {
                AllFiles = new ObservableCollection<RawFileData>(
                    FileNameHelpers.GetFiles(r).Select(x => new RawFileData(x)).OrderBy(x => x.TvFileInfo.Name));
            }
            else AllFiles = new ObservableCollection<RawFileData>();

            LoadSearch = new RelayCommand(DoLoadSearch, o => true);
            RenameTvShow = new RelayCommand(DoRenameTvShow, o => SelectedFile != null);
            RegetDetails = new RelayCommand(DoRegetDetails, o=>true);
        }

        private async void DoRegetDetails(object obj)
        {
            if (SSeriesDetails == null) return;

            var a = new WebInterface("");
            var ms = await a.GetMirrors();
            if (ms != null)
            {
                var m = ms.FirstOrDefault();

                SSeriesDetails = a.SaveFullSeriesById(ColTvDirectory, SSeriesDetails.GetSeriesId(), m);

            }
            
        }

        public Episode SelectedEpisode
        {
            get { return _selectedEpisode; }
            set
            {
                _selectedEpisode = value;
                SeasonInput = _selectedEpisode.SeasonNumber;
                Episode1Input = _selectedEpisode.Number;
            }
        }

        public RelayCommand RegetDetails { get; set; }

        public RelayCommand RenameTvShow { get; set; }

        public int SeasonInput
        {
            get { return _seasonInput; }
            set
            {
                _seasonInput = value;
                UpdateName();
            }
        }

        public string NextName { get; set; }

        public int Episode1Input
        {
            get { return _episode1Input; }
            set
            {
                _episode1Input = value;
                UpdateName();
            }
        }

        public int Episode2Input
        {
            get { return _episode2Input; }
            set
            {
                _episode2Input = value;
                UpdateName();
            }
        }

        public int Episode3Input
        {
            get { return _episode3Input; }
            set
            {
                _episode3Input = value;
                UpdateName();
            }
        }

        public int Episode4Input
        {
            get { return _episode4Input; }
            set
            {
                _episode4Input = value;
                UpdateName();
            }
        }

        public RawFileData SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                SeasonInput = _selectedFile.SeasonNumber;
                Episode1Input = Episode2Input = Episode3Input = Episode4Input = -1;
                if (_selectedFile.EpisodeNumbers.Count > 0) Episode1Input = _selectedFile.EpisodeNumbers[0];
                if (_selectedFile.EpisodeNumbers.Count > 1) Episode2Input = _selectedFile.EpisodeNumbers[1];
                if (_selectedFile.EpisodeNumbers.Count > 2) Episode3Input = _selectedFile.EpisodeNumbers[2];
                if (_selectedFile.EpisodeNumbers.Count > 3) Episode4Input = _selectedFile.EpisodeNumbers[3];
            }
        }

        public RelayCommand LoadSearch { get; set; }

        public string ColTvDirectory { get; set; }

        public SeriesDetails SSeriesDetails { get; set; }

        public bool HasData { get; set; }

        public ObservableCollection<RawFileData> AllFiles { get; set; }

        public string TestText { get; set; }
        public IEnumerable<string> AllDirs { get; set; }
        public Dictionary<string, List<DirectoryInfo>> AllCollected { get; set; }

        private void DoRenameTvShow(object obj)
        {
            var ru = Path.Combine(ColTvDirectory, "S" + SeasonInput.ToString("00"));
            if (!Directory.Exists(ru)) Directory.CreateDirectory(ru);
            ru = Path.Combine(ru, NextName);
            SelectedFile.TvFileInfo.MoveTo(ru);
            if (File.Exists(ru)) AllFiles.Remove(SelectedFile);
        }

        private void MakeDirectoryTree()
        {
            var l = new List<DirectoryInfo>();
            for (var i = 1; i < 10; i++)
            {
                var d = new DirectoryInfo(@"q:\MyTv.Library\" + i.ToString("000"));
                if (d.Exists) l.AddRange(d.GetDirectories("*.*"));
            }
            AllDirs = l.Select(x => x.Name).OrderBy(x => x).Distinct(StringComparer.CurrentCultureIgnoreCase);
            AllCollected = l.GroupBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                .ToDictionary(x => x.Key, y => y.ToList());
        }

        private void UpdateName()
        {
            var sb = new StringBuilder();
            var sb1 = new List<string>();
            var sb2 = new List<string>();
            sb.Append($"{SSeriesDetails.Series.Name.ToCleanString()} - S{SeasonInput.ToString("00")}");
            if (Episode1Input > -1)
            {
                sb1.Add(Episode1Input.ToString("00"));
                sb2.Add(
                    SSeriesDetails.Series.Episodes.FirstOrDefault(
                        x => x.SeasonNumber == SeasonInput && x.Number == Episode1Input)?.Name);
            }

            if (Episode2Input > -1)
            {
                sb1.Add(Episode2Input.ToString("00"));
                sb2.Add(
                    SSeriesDetails.Series.Episodes.FirstOrDefault(
                        x => x.SeasonNumber == SeasonInput && x.Number == Episode2Input)?.Name);
            }

            if (Episode3Input > -1)
            {
                sb1.Add(Episode3Input.ToString("00"));
                sb2.Add(
                    SSeriesDetails.Series.Episodes.FirstOrDefault(
                        x => x.SeasonNumber == SeasonInput && x.Number == Episode3Input)?.Name);
            }

            if (Episode4Input > -1)
            {
                sb1.Add(Episode4Input.ToString("00"));
                sb2.Add(
                    SSeriesDetails.Series.Episodes.FirstOrDefault(
                        x => x.SeasonNumber == SeasonInput && x.Number == Episode4Input)?.Name);
            }
            sb.Append("E" + string.Join("E", sb1));
            sb.Append(" - ");
            sb.Append(string.Join(", ", sb2).ToCleanString());
            sb.Append(SelectedFile.TvFileInfo.Extension.ToLower());
            NextName = sb.ToString();
        }

        private async void DoLoadSearch(object obj)
        {
            SSeriesDetails = null;

            if (!AllCollected.ContainsKey(TestText))
            {
                var client = new WebInterface("", @"k:\TempTvData");
                var mirs = await client.GetMirrors();
                var mir = mirs.FirstOrDefault();
                var res = await client.GetSeriesByName(TestText, mir);
                if (res != null && res.Any())
                {
                    var re = res.FirstOrDefault();
                    if (re != null)
                    {
                        var dir = "001";
                        if (re.Status == "Ended") dir = "002";
                        var wd = $@"q:\MyTv.Library\{dir}\{re.Name.ToCleanString()}";
                        if (!Directory.Exists(wd)) Directory.CreateDirectory(wd);
                        client.SaveFullSeriesById(wd, re.SeriesId, mir);
                        MakeDirectoryTree();
                    }
                }
                else
                {
                    return;
                }
            }
            var rr = AllCollected[TestText];
            var dname = rr?.FirstOrDefault();
            var temptvdir = "k" + dname.FullName.Substring(1);
            if (!Directory.Exists(temptvdir)) Directory.CreateDirectory(temptvdir);
            ColTvDirectory = temptvdir;
            var fdata = dname.GetFiles("tvdb_*.zip");
            if (!fdata.Any()) return;
            var fd = fdata.FirstOrDefault();
            var dt = WebInterface.GetFullSeriesByFile(fd.FullName);
            if (dt == null) return;
            SSeriesDetails = dt;
            HasData = true;
        }
    }
}