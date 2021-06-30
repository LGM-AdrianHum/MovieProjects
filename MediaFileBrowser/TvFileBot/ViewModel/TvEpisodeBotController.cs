using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PropertyChanged;
using Relays;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class TvEpisodeBotController
    {
        public TvEpisodeBotController()
        {
            WorkingDirectory = @"k:\BTSync.Archive\tv\";
            AllFilesInSync = new ObservableCollection<ItemBase>();
            _bgw = new BackgroundWorker();
            RescanWorkingDirectory = new RelayCommand(DoRescanWorkingDirectory, o => !_bgw.IsBusy);
            GetTvDetails = new RelayCommand(DoGetTvDetails, o => true);
            MoveVideoFile = new RelayCommand(DoMoveVideoFile, o => true);
            SearchDataContext = new SearchResultViewController();
            ShowSearch = new RelayCommand(DoShowSearch, o => true);
            RenameVideoFile=new RelayCommand(DoRenameVideoFile,o=>true);
            _dictShows = new Dictionary<string, LibraryDirectory>();
        }

        public RelayCommand RenameVideoFile { get; set; }


        private void DoMoveVideoFile(object obj)
        {
           
            var o = obj as ItemBase;
            if (o == null) return;
            MoveTargetFileDialog=new MoveFileDetails(o);
            MoveTargetFileDialog.MoveLocalLibrary();
            AllFilesInSync.Remove(o);

        }

        private void DoRenameVideoFile(object obj)
        {
            var o = obj as ItemBase;
            if (o == null) return;
            MoveTargetFileDialog = new MoveFileDetails(o);
            MoveTargetFileDialog.Rename();
        }
        public bool IsMoveFileReady { get; set; }

        public MoveFileDetails MoveTargetFileDialog { get; set; }

        public RelayCommand MoveVideoFile { get; set; }

        private void DoShowSearch(object obj)
        {
            IsSearchVisible = !IsSearchVisible;
        }

        public bool IsSearchVisible { get; set; }

        public RelayCommand ShowSearch { get; set; }

        public SearchResultViewController SearchDataContext { get; set; }

        private void DoGetTvDetails(object obj)
        {
            var o = obj as ItemBase;
            if (o == null) return;
            SearchDataContext.SearchString = o.SearchShowName;
            SearchDataContext.ItemBase = o;
            IsSearchVisible = true;
        }

        public RelayCommand GetTvDetails { get; set; }

        private void DoRescanWorkingDirectory(object obj)
        {
            _bgw.DoWork += (ss, ee) =>
            {
                ee.Result = Rescan();
            };
            _bgw.RunWorkerCompleted += (ss, ee) =>
            {
                var res = ee.Result as IEnumerable<ItemBase>;
                if (res == null) return;
                var itemBases = res as IList<ItemBase> ?? res.ToList();
                AllFilesInSync = new ObservableCollection<ItemBase>(itemBases);
                var ll = itemBases.DistinctBy(x => x.SearchShowName).Where(x => !string.IsNullOrEmpty(x.SearchShowName));
                DistinctShows = new ObservableCollection<ItemBase>(ll.OrderBy(x => x.Name));
            };
            _bgw.RunWorkerAsync();
        }

        public ObservableCollection<ItemBase> DistinctShows { get; set; }
        private Dictionary<string, LibraryDirectory> _dictShows { get; set; }
        public RelayCommand RescanWorkingDirectory { get; set; }

        public string WorkingDirectory { get; set; }
        private BackgroundWorker _bgw;
        public IEnumerable<ItemBase> Rescan()
        {

            if (ListOfShow == null)
            {
                ListOfShow = new List<LibraryDirectory>();
                for (var i = 1; i < 10; i++)
                {
                    var qlib = Path.Combine(@"q:\MyTv.Library\", i.ToString("000"));
                    if (!Directory.Exists(qlib)) continue;
                    var ldir = Directory.GetDirectories(qlib).Select(x => new LibraryDirectory(x));
                    ListOfShow.AddRange(ldir);
                }
            }

            var r = Directory.GetFiles(WorkingDirectory, "*.*", SearchOption.AllDirectories).Where(x => x.IsMovie()).Select(x => new ItemBase(x)).ToList();
            //^(?<showname>.*?)(?<season>[Ss][0-9]{1,2})(?<episode>[xXeE&\-][0-9]{1,2}){1,3}(?<after>.*)
            var rx1 =
                new Regex(
                    @"^(?<showname>.*?)(?<season>[Ss][0-9]{1,2})(?<episode>[xXeE&\-][0-9]{1,2}){1,3}(?<after>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            foreach (var rr in r)
            {
                var flag = rr.ParseWithRegex(rx1);
                if (flag)
                {
                    var l = ListOfShow.FirstOrDefault(x => rr.SearchKey == x.Key);
                    if (l != null)
                    {
                        rr.MatchingShow = l;
                        if (!_dictShows.ContainsKey(l.Key)) _dictShows.Add(l.Key, l);
                    }
                    else
                    {
                        NoDirectory = true;
                    }
                }
            }

            foreach (var k in _dictShows.Values)
            {
                k.LoadDetails();
            }

            return r;
        }

        public bool NoDirectory { get; set; }

        public List<LibraryDirectory> ListOfShow { get; set; }

        public ObservableCollection<ItemBase> AllFilesInSync { get; set; }
    }
}
