using System;
using System.Collections.ObjectModel;
using System.Linq;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using PropertyChanged;
using Relays;

namespace EpisodeViewer.ViewModel
{
    [ImplementPropertyChanged]
    public class GoogleSearcherVm
    {
        private GoogleResult _googleSelected;
        private MainVm _parent;

        public GoogleSearcherVm()
        {
            
            ExecuteQuery = new RelayCommand(DoExecuteQuery, o => true);
            UpdateParent=new RelayCommand(DoUpdateParent, o=>true);
        }

        public void SetParent()
        {
            
        }

        private void DoUpdateParent(object obj)
        {
            _parent.SetId(SelectedId);
        }

        public RelayCommand UpdateParent { get; set; }

        public RelayCommand ExecuteQuery { get; set; }

        public string QueryString { get; set; }

        private void DoExecuteQuery(object obj)
        {
            //AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY
            //tvdb:003372514794118234263:rrgjqqhjsj8
            //tmdb:003372514794118234263:36td8wej70u
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:rrgjqqhjsj8";

            CustomsearchService customSearchService =
                new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() {ApiKey = apiKey});
            Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = customSearchService.Cse.List(QueryString);
            listRequest.Cx = searchEngineId;
            Search search = listRequest.Execute();
            AllGoogleResult = new ObservableCollection<GoogleResult>(
                search.Items.Select(x => new GoogleResult(x))
                    //.Where(x => !string.IsNullOrEmpty(x.Id) && !string.IsNullOrEmpty(x.Title))
                    .OrderBy(x => x.Title).ToList());
        }

        public ObservableCollection<GoogleResult> AllGoogleResult { get; set; }

        public GoogleResult GoogleSelected
        {
            get { return _googleSelected; }
            set
            {
                _googleSelected = value;
                SelectedId = value.Id;
            }
        }

        public string SelectedId { get; set; }
        
        internal void SetParent(MainVm mainVm)
        {
            _parent = mainVm;
        }
    }
}