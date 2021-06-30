using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Media;
using Google.Apis.Customsearch.v1;
using Relays;
using TvFileBot.Controls;

namespace TvFileBot.ViewModel
{
    public class FileObject
    {
        public enum VideoType
        {
            Unknown,
            Tv,
            Movie
        }

        public FileObject()
        {
        }

        public FileObject(string s)
        {
            Fullname = s;
            Name = Path.GetFileName(s);
            FileObjectType = VideoType.Unknown;

            if (File.Exists(s))
            {
                var fi = new FileInfo(s);
                FileLength = fi.Length;
                FileCreationTime = fi.CreationTime;
                FileExtension = fi.Extension;
                DirectoryName = fi.DirectoryName;
            }
            IsMovie = s.IsMovie();
            if (string.IsNullOrEmpty(Name) || !IsMovie) return;
            var rx =
                new Regex(
                    @"^(?<showname>.+?).([Ss]?)(?<seasonnumber>[0-9]{1,2})([xXeE](?<episodenamenumber>[0-9]{2}?)){1,5}");
            var mt = rx.Match(Name);
            if (mt.Success)
            {
                FileObjectType = VideoType.Tv;
                ImageSource = ImageHelpers.CreateImageSource("pack://application:,,,/Images/vid_tv.ico", true);
                Showname = Regex.Replace(mt.Groups["showname"].Value.TrimEnd('-',' '), @"\.", " ");
                SeasonNumber = mt.Groups["seasonnumber"].Value;
                Episodes = mt.Groups["episodenamenumber"].Captures.Cast<Capture>().Select(x => x.Value).Distinct().ToList();
                Episode = Episodes.FirstOrDefault();
                return;
            }

            var rp = (MovieNameHelper.MovieParse(Name) ?? MovieNameHelper.MovieParse2(Name)) ??
                     MovieNameHelper.MovieParse3(Name);
            if (rp != null)
            {
                Showname = rp.Movie;
                var sy = rp.Year;
                var syi = 0;
                if (!string.IsNullOrEmpty(sy) && int.TryParse(sy, out syi)) Showyear = syi;
                FileObjectType = VideoType.Movie;
                ImageSource = ImageHelpers.CreateImageSource("pack://application:,,,/Images/vid_movie.ico", true);
                return;
            }
            ImageSource = ImageHelpers.CreateImageSource("pack://application:,,,/Images/vid_video.ico", true);
            SelectFileObject = new RelayCommand(DoSelectFileObject, o => true);
        }

        public string DirectoryName { get; set; }

        private void DoSelectFileObject(object obj)
        {
            MessageBox.Show("Obj");
        }

        public RelayCommand SelectFileObject { get; set; }


        public int Showyear { get; set; }

        public bool IsMovie { get; set; }

        public ImageSource ImageSource { get; set; }

        public string FileExtension { get; set; }

        public DateTime FileCreationTime { get; set; }

        public long FileLength { get; set; }

        public VideoType FileObjectType { get; set; }

        public string Episode { get; set; }

        public List<string> Episodes { get; set; }

        public string SeasonNumber { get; set; }

        public string Showname { get; set; }

        public string Name { get; set; }

        public string Fullname { get; set; }

        public string FileLengthStr => FileLength.ToBytes();

        public string FileCreationTimeStr => FileCreationTime.ToString("dd-MMM-yyyy HH:mm:ss");

        public string SeasonCodes
            =>
                string.IsNullOrEmpty(SeasonNumber) || Episodes == null
                    ? ""
                    : $"S{SeasonNumber.PadLeft(2,'0')}E{string.Join("E", Episodes.Select(x=>x.PadLeft(2, '0')))}";
    }

    public class TvFileObject: FileObject
    {
        public void GetSeriesIdFromGoogle()
        {
            const string apiKey = "AIzaSyBYHmmTL9b-7Jw3SrVYlxYjKBn_D8UneeY";
            const string searchEngineId = "003372514794118234263:rrgjqqhjsj8";
            
            var customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            var listRequest = customSearchService.Cse.List(Showname);
            listRequest.Cx = searchEngineId;
            var search = listRequest.Execute();
            foreach (var item in search.Items)
            {

                var myUri = new Uri(item.Link);
                var param1 = HttpUtility.ParseQueryString(myUri.Query).Get("id");
                if (!string.IsNullOrEmpty(param1))
                {
                    var id = 0;
                    if (int.TryParse(param1, out id))
                    {
                        TvDbId = id;
                        break;
                    }
                }
            }
            Console.WriteLine(TvDbId);
        }

        public int TvDbId { get; set; }
    }
}