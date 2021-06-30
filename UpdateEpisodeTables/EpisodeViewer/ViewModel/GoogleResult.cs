using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Customsearch.v1.Data;
using PropertyChanged;

namespace EpisodeViewer.ViewModel
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
                var p =new Uri(r.Link);
                Id=HttpUtility.ParseQueryString(p.Query).Get("seriesid");
                if (string.IsNullOrEmpty(Id)) Id = HttpUtility.ParseQueryString(p.Query).Get("id");
            }

            Title = r.Title;
            Snippet = r.Snippet.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Replace("  ", " ");
        }

        //public GoogleResult(SearchMovie sc)
        //{
        //    Link = $"https://www.themoviedb.org/movie/{sc.Id}";
        //    Title = $"{sc.Title} ({sc.ReleaseDate?.Year.ToString("0000")})";
        //    Snippet = sc.Overview;
        //    Id = sc.Id.ToString();
        //}

        public string Snippet { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        public string Link { get; set; }
    }
}

