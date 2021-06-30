using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Web;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using PropertyChanged;
using Relays;
using TVDB.Web;

namespace TvDbSearchControl
{
    [ImplementPropertyChanged]
    public class SearchClass
    {
        public enum SearchMode
        {
            TvDb,
            Google
        }

        private readonly WebInterface _client;
        private readonly ReaderWriterLock _sync = new ReaderWriterLock();

        public SearchClass()
        {
            _client = new WebInterface("", @"q:\MyTv.Library\");
            ExecuteSearch = new RelayCommand(DoExecuteSearch, o => !string.IsNullOrEmpty(SearchString));
        }


        public string SearchString { get; set; }

        public RelayCommand ExecuteSearch { get; set; }

        public SearchMode Mode { get; set; }

        public ObservableCollection<ResultItem> AllResults { get; set; }
        public ResultItem SelectedResult { get; set; }


        private async void DoExecuteSearch(object obj)
        {
            Mode = obj.ToString() == "g" ? SearchMode.Google : SearchMode.TvDb;

            if (Mode == SearchMode.TvDb)
            {
                if (string.IsNullOrEmpty(SearchString)) return;
                var mirrors = await _client.GetMirrors();
                if (mirrors == null || !mirrors.Any()) return;
                var mirror = mirrors.FirstOrDefault();
                if (mirror == null) return;
                var res = await _client.GetSeriesByName(SearchString, mirror);
                var rest = res.Select(x => new ResultItem(x));
                UpdateValueFromResultList(rest);
            }
            else
            {
                var ik = new List<ResultItem>();
                const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
                const string searchEngineId = "003372514794118234263:rrgjqqhjsj8";
                var customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = apiKey });
                var listRequest = customSearchService.Cse.List(SearchString);
                listRequest.Cx = searchEngineId;
                var search = listRequest.Execute();
                foreach (var item in search.Items)
                {
                    var myUri = new Uri(item.Link);
                    var param1 = HttpUtility.ParseQueryString(myUri.Query).Get("id");
                    if (string.IsNullOrEmpty(param1)) continue;
                    var ij = 0;
                    int.TryParse(param1, out ij);
                    var r = new ResultItem
                    {
                        ShowName = item.Title,
                        OverView = item.Snippet.Replace("\r\n"," ").Replace("\r"," ").Replace("\n"," ").Replace("  "," "),
                        Id = ij,
                        Poster = "",
                        FirstAired = DateTime.MinValue,
                        Status = "?"
                    };
                    ik.Add(r);
                }
                UpdateValueFromResultList(ik);
            }
        }

        private void UpdateValueFromResultList(IEnumerable<ResultItem> rest)
        {
            _sync.AcquireWriterLock(Timeout.Infinite);
            try
            {
                AllResults = new ObservableCollection<ResultItem>(rest.ToList());
                SelectedResult = AllResults.FirstOrDefault();
            }
            finally
            {
                _sync.ReleaseWriterLock();
            }
        }
    }
}