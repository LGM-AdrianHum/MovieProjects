using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PropertyChanged;
using Relays;
using TVDB.Web;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class SearchResultViewController
    {
        public SearchResultViewController()
        {
            ExecuteSearch = new RelayCommand(DoExecuteSearch, o => true);
            AcceptSearch = new RelayCommand(DoAcceptSearch, o => true);
        }

        public SearchItem SelectSearchResult { get; set; }

        private async void DoAcceptSearch(object obj)
        {
            ItemBase.TvDbId = SelectSearchResult.Id;
            
            if(ItemBase.MatchingShow==null) ItemBase.MatchingShow=new LibraryDirectory();
            ItemBase.MatchingShow.Id = SelectSearchResult.Id;
            ItemBase.MatchingShow.Name = SelectSearchResult.Title;
            var fp=ItemBase.MatchingShow.FullName;
            if (string.IsNullOrEmpty(fp))
            {
                fp = "Q:\\CacheData";
            }
            var client = new WebInterface(null, @"q:\");
            var mirs = await client.GetMirrors();
            if (mirs == null || !mirs.Any()) return;
            ItemBase.MatchingShow.SeriesInformation=client.SaveFullSeriesById(fp, SelectSearchResult.Id, mirs.FirstOrDefault());
            var st = ItemBase.MatchingShow.Status = ItemBase.MatchingShow.SeriesInformation.Series.Status;
            var dr = "002";
            var shw = ItemBase.MatchingShow.SeriesInformation.Series.Name.ToCleanString();
            var id = ItemBase.MatchingShow.Id;
            if (st.Substring(0, 4).Equals("cont", StringComparison.InvariantCultureIgnoreCase)) dr = "001";

            var newpath = $@"q:\MyTv.Library\{dr}\{shw}";
            var datafile = $"tvdb_{id}.zip";
            if (!Directory.Exists(newpath)) Directory.CreateDirectory(newpath);
            if(File.Exists(Path.Combine(newpath, datafile))) File.Delete(Path.Combine(newpath, datafile));
            File.Move(Path.Combine(fp,datafile), Path.Combine(newpath, datafile));
            ItemBase.MatchingShow.FullName = newpath;
            ItemBase.MatchingShow.DriveNumber = dr;
        }

        public RelayCommand AcceptSearch { get; set; }

        private async void DoExecuteSearch(object obj)
        {
            var client = new WebInterface(null, "Q:\\");
            var mirs = await client.GetMirrors();
            if (mirs == null || !mirs.Any()) return;
            var w = await client.GetSeriesByName(SearchString, mirs.FirstOrDefault());
            AllSearchResults = new ObservableCollection<SearchItem>(w.Select(x => new SearchItem(x)));
            SelectSearchResult = AllSearchResults.FirstOrDefault();
        }

        public ObservableCollection<SearchItem> AllSearchResults { get; set; }

        public RelayCommand ExecuteSearch { get; set; }

        public SearchResultViewController(string ss)
        {
            SearchString = ss;
            TvDbId = 0;
        }
        public string SearchString { get; set; }
        public int TvDbId { get; set; }
        public ItemBase ItemBase { get; set; }
    }
}