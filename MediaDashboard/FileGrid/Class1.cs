using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using PropertyChanged;
using Relays;
using TVDB.Model;
using TVDB.Utility;
using TVDB.Web;
using SearchOption = System.IO.SearchOption;

namespace FileGrid
{
    [ImplementPropertyChanged]
    public class FileViewModel
    {
        private WebInterface _client;
        private Mirror _mirror;
        private ParseValue _selectedFileItem;
        private Series _selectedResult;

        public FileViewModel()
        {
            var di = new DirectoryInfo(@"k:\\BtSync.Archive\tv");
            if (!di.Exists) return;
            var afl = di.GetFiles("*.*", SearchOption.AllDirectories).AsParallel()
                .Where(x => x.Name.IsMovie())
                .Select(x => new ParseValue {MediaFile = x})
                .ToList();
            foreach (var af in afl)
            {
                af.ParseTv();
            }
            AlLFileList = new ObservableCollection<ParseValue>(afl);
            TvDbIdCache = new Dictionary<string, int>();
            var n = new DirectoryInfo(@"Q:\MyTv.Library");
            AllDirectories = new List<DirectoryInfo>();
            foreach (var l in n.GetDirectories())
            {
                AllDirectories.AddRange(l.GetDirectories());
            }

            
            ProcessTvData = new RelayCommand(DoProcessTvData, o => SelectedFileItem != null);
            GetShow = new RelayCommand(DoGetShow, o => !string.IsNullOrEmpty(SelectedResultId));
            MoveFile = new RelayCommand(DoMoveFile, o=>SelectedFileItem!=null);

        }

        private void DoMoveFile(object obj)
        {
            var fd = AllDirectories.FirstOrDefault(x => x.Name == ActualSeries.Series.Name.ToCleanString());
            if (fd == null)
            {
                var sss = ActualSeries.Series.Status ?? "Ended";
                var r =
                    new DirectoryInfo(
                        Path.Combine(@"k:\MyTvLib", sss,
                            ActualSeries.Series.Name.ToCleanString(), "S"+SelectedFileItem.SeasonInt.ToString("00")));
                if (!r.Exists) r.Create();
                fd = r;
            }
            else
            {
                fd = new DirectoryInfo(Path.Combine(fd.FullName, "S" + SelectedFileItem.SeasonInt.ToString("00")));
            }
            if (!fd.Exists) fd.Create();

            FileSystem.MoveFile(SelectedFileItem.MediaFile.FullName, Path.Combine(fd.FullName, SelectedFileItem.NewFileName),
                UIOption.AllDialogs);

            AlLFileList.Remove(SelectedFileItem);
        }

        public RelayCommand MoveFile { get; set; }

        public List<DirectoryInfo> AllDirectories { get; set; }

        public RelayCommand GetShow { get; set; }

        public List<Series> AllResults { get; set; }

        public Series SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                _selectedResult = value;
                SelectedResultId = value.SeriesId.ToString();
            }
        }

        public string SelectedResultId { get; set; }
        public SeriesDetails ActualSeries { get; set; }
        public RelayCommand ProcessTvData { get; set; }

        public Dictionary<string, int> TvDbIdCache { get; set; }

        public ObservableCollection<ParseValue> AlLFileList { get; set; }

        public ParseValue SelectedFileItem
        {
            get { return _selectedFileItem; }
            set
            {
                _selectedFileItem = value;
                DoProcessTvData(null);
                if(!string.IsNullOrEmpty(SelectedResultId)) DoGetShow(null);
            }
        }

        public string SearchName { get; set; }

        private async void DoGetShow(object obj)
        {
            int i;
            if (!int.TryParse(SelectedResultId, out i)) return;
            if (_client == null || _mirror == null)
            {
                _client = new WebInterface("", "k:\\CacheData");
                _mirror = (await _client.GetMirrors()).FirstOrDefault();
                if (_mirror == null) return;
            }
            ActualSeries = await _client.GetFullSeriesById(i, _mirror);
            SelectedFileItem?.SetFilename(ActualSeries);
        }

        private async void DoProcessTvData(object obj)
        {
            if (SelectedFileItem == null) return;
            if (obj == null)
            {
                var l = SelectedFileItem.Title;
                if (string.IsNullOrEmpty(l)) l = SelectedFileItem.MediaFile.Name;
                SearchName = l;
            }
            else
            {
                _client = new WebInterface("", "k:\\CacheData");
                _mirror = (await _client.GetMirrors()).FirstOrDefault();
                if (_mirror == null) return;
                AllResults = (await _client.GetSeriesByName(SearchName, _mirror)).ToList();
            }
        }
    }
}