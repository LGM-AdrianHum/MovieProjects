using System;
using System.Web;
using Google.Apis.Customsearch.v1.Data;

namespace TVDB.MyGoogle
{
    /// <summary>
    /// 
    /// </summary>
    public class UniversalResultSet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public UniversalResultSet(Result x)
        {
            Link = x.Link;
            var myUri = new Uri(x.Link);
            var param1 = HttpUtility.ParseQueryString(myUri.Query).Get("id");
            var id = 0;
            if (!string.IsNullOrEmpty(param1)&& (int.TryParse(param1, out id))) Id = id;
            Name = x.Title;
            Overview = x.Snippet.Replace("\r", "").Replace("\n", " ").Replace("  ", " ").Replace("  ", " ");


        }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Overview { get; set; }
    }
}
