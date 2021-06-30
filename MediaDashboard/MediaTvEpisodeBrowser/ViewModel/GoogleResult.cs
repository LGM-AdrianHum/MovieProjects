using System;
using System.IO;
using System.Text;
using System.Web;
using Google.Apis.Customsearch.v1.Data;
using PropertyChanged;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MediaTvEpisodeBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class GoogleResult
    {
        public GoogleResult(Result r)
        {
            Link = r.Link;
            var l = Path.GetFileName(Link);
            if (l != null)
            {
                var p = l.IndexOf("-", StringComparison.Ordinal);
                if (p < 0) return;
                Id = l.Substring(0, p);
                Title = r.Title;
                Snippet = r.Snippet.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Replace("  ", " ");
            }
        }
        
        public GoogleResult(SearchMovie sc)
        {
            Link = $"https://www.themoviedb.org/movie/{sc.Id}";
            Title = $"{sc.Title} ({sc.ReleaseDate?.Year.ToString("0000")})";
            Snippet = sc.Overview;
            Id = sc.Id.ToString();
        }

        public string Snippet { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        public string Link { get; set; }
    }
}
