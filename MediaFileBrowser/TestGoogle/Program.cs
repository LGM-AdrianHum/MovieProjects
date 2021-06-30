using System;
using System.Web;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;

namespace TestGoogle
{
    class Program
    {
        static void Main(string[] args)
        {//AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY
         //tvdb:003372514794118234263:rrgjqqhjsj8
         //tmdb:003372514794118234263:36td8wej70u
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:36td8wej70u";
            const string query = "the chipmunk adventure";
            CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            Google.Apis.Customsearch.v1.CseResource.ListRequest listRequest = customSearchService.Cse.List(query);
            listRequest.Cx = searchEngineId;
            Search search = listRequest.Execute();
            foreach (var item in search.Items)
            {
                
                Uri myUri = new Uri(item.Link);
                string param1 = HttpUtility.ParseQueryString(myUri.Query).Get("id");
                
                Console.WriteLine($"Title : {item.Title}\r\n{item.Link}\r\n{item.Snippet}\r\n{param1}\r\n");
            }
            Console.ReadLine();
        }
    }
}
