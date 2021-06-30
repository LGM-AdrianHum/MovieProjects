using System.Collections.Generic;
using System.Linq;
using Google.Apis.Customsearch.v1;

namespace TVDB.MyGoogle
{
    /// <summary>
    /// 
    /// </summary>
    public static class GoogleFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<UniversalResultSet> GoogleResults(string query)
        {
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:36td8wej70u";
            
            var customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            var listRequest = customSearchService.Cse.List(query);
            listRequest.Cx = searchEngineId;
            var search = listRequest.Execute();
            return search?.Items.AsParallel().Select(x => new UniversalResultSet(x)).ToList();
        }
    }
}