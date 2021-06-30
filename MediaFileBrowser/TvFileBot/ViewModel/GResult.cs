using System.Text.RegularExpressions;
using Google.Apis.Customsearch.v1.Data;
using PropertyChanged;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class GResult
    {
        private readonly Result _result;
        public GResult() { }
        public GResult(Result r)
        {
            _result = r;
            var rx = new Regex(@"\/(?<tmdbid>\d{2,9})-(?<title>.*)");
            var tm = rx.Match(r.Link);
            if (tm.Success)
            {
                var s = tm.Groups["tmdbid"].Value;
                var ii = 0;
                if (int.TryParse(s, out ii)) TmDbId = ii;
            }
        }

        public int TmDbId { get; set; }
        public string Title => _result?.Title;
        public string Link => _result?.Link;
        public string Snippet => _result?.Snippet.Replace("\n", " ").Replace("\r", " ").Replace("  ", " ");
    }
}