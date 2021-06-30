using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using PropertyChanged;
using TvFileBot.Controls;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class MovieItem
    {
        public MovieItem()
        {
        }

        public MovieItem(FileInfo fileInfo)
        {
            ListBoxItemVisible = true;
            Fullname = fileInfo.FullName;
            Filename = fileInfo.Name;
            SearchFilename = "Unknown Movie";
            DirectoryName = Path.GetFileName(fileInfo.DirectoryName);
            var rp = (MovieNameHelper.MovieParse(Filename) ?? MovieNameHelper.MovieParse2(Filename)) ??
                     MovieNameHelper.MovieParse3(Filename);
            if (rp != null)
            {
                var sy = rp.Year;
                var syi = 0;
                if (!string.IsNullOrEmpty(sy) && int.TryParse(sy, out syi)) Showyear = syi;
                SearchFilename = $"{rp.Movie} ({rp.Year})";
                ImageSource = ImageHelpers.CreateImageSource("pack://application:,,,/Images/vid_movie.ico", true);
            }

            rp = (MovieNameHelper.MovieParse(DirectoryName) ?? MovieNameHelper.MovieParse2(DirectoryName)) ??
                 MovieNameHelper.MovieParse3(DirectoryName);
            if (rp != null)
            {
                SearchDirectoryName = $"{rp.Movie} ({rp.Year})";
            }
            DisplayDirectory = !string.IsNullOrEmpty(SearchDirectoryName) && (SearchDirectoryName != SearchFilename);
            QueryByFilename = !DisplayDirectory;
            //NameToUse = SearchFilename;
        }

        public IList<GResult> AllGoogleResults { get; set; }

        public GResult GoogleSearchResult { get; set; }

        public bool ListBoxItemVisible { get; set; }

        public bool SearchFailed { get; set; }
        public string NameToUse { get; set; }
        public bool DisplayDirectory { get; set; }
        public int Showyear { get; set; }
        public ImageSource ImageSource { get; set; }

        public string Fullname { get; set; }
        public string Filename { get; set; }
        public string DirectoryName { get; set; }
        public string SearchFilename { get; set; }
        public string SearchDirectoryName { get; set; }
        public bool QueryByFilename { get; set; }
        public bool QueryByDirectoryName { get; set; }

        public int TmDbId { get; set; }
        public string TmdbName { get; set; }
        public bool ExecSearch { get; set; }

        public override string ToString()
        {
            return SearchFilename;
        }

        public void DoGoogleQuery()
        {
            ExecSearch = true;
            NameToUse = "...";
            AllGoogleResults = null;
            GoogleSearchResult = null;

            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:36td8wej70u";
            var customSearchService = new CustomsearchService(new BaseClientService.Initializer {ApiKey = apiKey});
            var listRequest = customSearchService.Cse.List(SearchFilename);
            listRequest.Cx = searchEngineId;
            var search = listRequest.Execute();

            if (search?.Items == null || (bool) !search?.Items.Any()) return;
            if (search?.Items == null || !search.Items.Any()) return;
            var lst =
                search.Items.AsParallel().Where(x => !x.Link.ToLower().Contains("/tv/")).Select(x => new GResult(x));
            AllGoogleResults = new ObservableCollection<GResult>(lst);
            GoogleSearchResult = lst.FirstOrDefault();
            NameToUse = GoogleSearchResult?.Title;
        }
    }
}