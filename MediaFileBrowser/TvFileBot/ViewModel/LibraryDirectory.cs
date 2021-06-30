using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PropertyChanged;
using TVDB.Model;
using TVDB.Web;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class LibraryDirectory
    {
        private readonly string _seriesfile;

        public LibraryDirectory(string a, string b)
        {
            FullName = a;
            Name = b;
            Status = "unknown";
        }

        public LibraryDirectory(string s)
        {
            Status = "unknown";

            FullName = s;
            var j = s.Split('\\');
            if (j.Any() && j.Length > 1) DriveNumber = j[2];
            var possiblefile = Directory.GetFiles(s, "tvdb_*.zip");
            if (possiblefile.Any())
            {
                var pf = possiblefile.FirstOrDefault();
                if (!string.IsNullOrEmpty(pf))
                {
                    pf = Path.GetFileName(pf);
                    var rx = new Regex(@"\d{1,99}");
                    var c = rx.Match(pf).Value;
                    var i = 0;
                    if (int.TryParse(c, out i)) Id = i;
                    if (i == 0) NeedsIdFile = true;
                    _seriesfile = Path.Combine(s, pf);
                }



            }
            Name = Path.GetFileName(s);
            var rgx = new Regex("[^a-zA-Z0-9]");
            Key = rgx.Replace(Name, "").ToLower();
        }

        public void LoadDetails()
        {
            Status = "No Data";
            if (string.IsNullOrEmpty(_seriesfile)) return;
            SeriesInformation = WebInterface.GetFullSeriesByFile(_seriesfile);
            Status = SeriesInformation.Series.Status;
        }

        public string Status { get; set; }

        public SeriesDetails SeriesInformation { get; set; }

        public string DriveNumber { get; set; }

        public bool NeedsIdFile { get; set; }

        public LibraryDirectory()
        {
        }

        public string Key { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Id { get; set; }
    }
}