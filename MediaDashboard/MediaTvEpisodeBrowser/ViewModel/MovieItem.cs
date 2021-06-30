using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using PropertyChanged;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using UserControls;
using UtilityFunctions;
using UtilityFunctions.Movie;

namespace MediaTvEpisodeBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class MovieItem
    {
        public MovieItem(FileInfo f)
        {
          
            MyFileInfo = f;
            FullName = f.FullName;
            FileName = f.Name;
            FileProperties = $"{f.Length.ToBytes()}, {f.CreationTime.ToString("dd-MMM-yyyy")}";
            Parse = (MovieNameHelper.MovieParse(FileName) ?? MovieNameHelper.MovieParse2(FileName)) ??
                    MovieNameHelper.MovieParse3(FileName);
            if (Parse != null)
            {
                QueryString = Parse.Movie;
                QueryYear = Parse.Year;
                GoogleSearch = Parse.MovieYear;
            }
            else
            {
                QueryString = FileName;
                QueryYear = "";
                GoogleSearch = FileName;
            }
            FinalName = QueryString;
            FinalYear = QueryYear;
            Overview = "Hit Search";
        }

        internal async void RenameAndMove(GoogleResult googleSelected)
        {
            var i = 0;
            if(!int.TryParse(googleSelected.Id,out i)) return;
            TmdbId = i;
            var client = new TMDbClient("125e475654c781837d575da127e4d7bd");
            var res = client.GetMovie(TmdbId,
                MovieMethods.AlternativeTitles |
                MovieMethods.Changes |
                MovieMethods.Credits |
                MovieMethods.Images |
                MovieMethods.Keywords |
                MovieMethods.Releases |
                MovieMethods.Similar |
                MovieMethods.Lists |
                MovieMethods.Reviews |
                MovieMethods.Undefined
                );
            if (res == null) return;

            var root = Path.GetPathRoot(FullName);
            if (root != null)
            {
                var extension = Path.GetExtension(FullName);
                var yr = res.ReleaseDate;
                var yrr = "1900";
                if (yr.HasValue) yrr = res.ReleaseDate?.Year.ToString("0000");
                var moviefolder = $"{res.Title.ToCleanString()} ({yrr})";
                var packandmove = Path.Combine(root, "PackAndMove", moviefolder);
                if (!Directory.Exists(packandmove)) Directory.CreateDirectory(packandmove);
                var ffname = Path.Combine(packandmove, moviefolder + extension);
                File.Move(FullName, ffname);
                FullName = ffname;
                FileName = Path.GetFileName(FullName);
                MyFileInfo=new FileInfo(FullName);
                packandmove = Path.GetDirectoryName(FullName);
                var nfo = new NfoMovie();
                nfo.LoadFrom(res);
                var nfofile = Path.Combine(packandmove, "movie.nfo");
                nfo.Save(nfofile);
                var ss = "";
                var pstf = await PrepareImage(res, packandmove, "w1280", res.BackdropPath);
                var ppp = Path.Combine(packandmove, "BackDrop.jpg");
                File.Copy(pstf, ppp);
                pstf = await PrepareImage(res, packandmove, "w500", res.PosterPath);
                ppp = Path.Combine(packandmove, "Poster.jpg");
                File.Copy(pstf, ppp);
                PosterSource = ImageHelpers.CreateImageSource(ppp);
                foreach(var rr in res.Images.Backdrops) await PrepareImage(res, packandmove, "w1280", rr.FilePath);
                foreach (var rr in res.Images.Posters) await PrepareImage(res, packandmove, "w500", rr.FilePath);

                if (res.BelongsToCollection != null && res.BelongsToCollection.Any())
                {
                    var belongsToCollection = res.BelongsToCollection.FirstOrDefault();
                    if (belongsToCollection != null)
                    {
                        var colname = "["+belongsToCollection.Name.ToCleanString()+"]";
                        var fcolpath = Path.Combine(root, "Packed.Collections", colname);
                        if (!Directory.Exists(fcolpath)) Directory.CreateDirectory(fcolpath);
                        fcolpath = Path.Combine(fcolpath, Path.GetFileName(packandmove));
                        Directory.Move(packandmove, fcolpath);
                       
                    }

                }
                else
                {
                    var defname = res.Title.Substring(0, 1).ToUpper();
                    var rx = new Regex("[A-Za-z]");
                    var l = rx.Match(defname);
                    if (!l.Success) defname = "#";
                    if (res.Title.StartsWith("The ", StringComparison.CurrentCultureIgnoreCase))
                        defname = "The";
                    defname = "MyMovies." + defname;
                    var fcolpath = Path.Combine(root, "Packed.Movies", defname);
                    if (!Directory.Exists(fcolpath)) Directory.CreateDirectory(fcolpath);
                    fcolpath = Path.Combine(fcolpath, Path.GetFileName(packandmove));
                    Directory.Move(packandmove, fcolpath);
                    
                }
            }
            
        }

        public SolidColorBrush BackColor { get; set; }

        private static async Task<string> PrepareImage(Movie res, string packandmove, string imagesize, string resourcename)
        {
            var wc = new WebClient();
            var pst = new Uri($"http://image.tmdb.org/t/p/{imagesize}/{resourcename}");
            var pstf = Path.Combine(packandmove, "extrafanart");
            pstf = Path.Combine(pstf, resourcename.TrimStart('/').Replace("/", "\\"));
            if (!Directory.Exists(Path.GetDirectoryName(pstf))) Directory.CreateDirectory(Path.GetDirectoryName(pstf));
            await wc.DownloadFileTaskAsync(pst, pstf);
            return pstf;
        }

        public ImageSource PosterSource { get; set; }


        public string FileProperties { get; set; }

        public string GoogleSearch { get; set; }

        public string QueryYear { get; set; }

        public string QueryString { get; set; }

        public ParseValue Parse { get; set; }

        public string FileName { get; set; }

        public string FullName { get; set; }

        public FileInfo MyFileInfo { get; set; }

        public string FinalName { get; set; }
        public string FinalYear { get; set; }
        public int TmdbId { get; set; }
        public string Overview { get; set; }
    }
}