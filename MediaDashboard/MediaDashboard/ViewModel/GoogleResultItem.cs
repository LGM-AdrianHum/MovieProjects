using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Google.Apis.Customsearch.v1.Data;

namespace MediaDashboard.ViewModel
{
    public class GoogleResultItem
    {
        private Result _res;

        public GoogleResultItem(Result x)
        {
            _res = x;
            Link = x.Link;
            Title = x.Title;
            Snippet = x.Snippet.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Replace("  ", " ");
            TmdbId = 0;
            TvdbId = 0;
            var myUri = new Uri(x.Link);
            var param2 = HttpUtility.ParseQueryString(myUri.Query).Get("id");
            var param1= HttpUtility.ParseQueryString(myUri.Query).Get("seriesid");
            var param = param1;
            if (string.IsNullOrEmpty(param)) param = param2;
            var i = 0;
            if (int.TryParse(param, out i)) TvdbId = i;

            var j = Path.GetFileName(x.Link);
            var rx = new Regex("\\d{1,99");
            var mt = rx.Match(j);
            if (!mt.Success) return;
            var s = mt.Value;
            i = 0;
            if (int.TryParse(s, out i)) TmdbId = i;
        }

        public int TmdbId { get; set; }
        public int TvdbId { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }


    }
}