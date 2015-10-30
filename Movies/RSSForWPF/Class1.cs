using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Linq;

namespace WPFRSS
{
    [ImplementPropertyChanged]
    public class RssFeedViewModel
    {
        public string FeedFormat { get; set; }
        public string Query { get; set; }
        private string _queryUri;
        public string FeedQuery { get; set; }
        public string ResultString { get; set; }
        public XDocument ResultXml { get; set; }
        public void CleanupQuery()
        {

            Regex rgx = new Regex("[^a-zA-Z0-9 -:]");
            var str = rgx.Replace(Query, " ");
            rgx = new Regex(@"[ ]{2,}");
            Query = rgx.Replace(str, @" ");
            _queryUri = Uri.EscapeDataString(Query);
            FeedQuery = string.Format(FeedFormat, _queryUri);

        }

        public void DoQuery()
        {
            var client = new WebClient();
            var responseStream = new GZipStream(client.OpenRead(new Uri(FeedQuery)), CompressionMode.Decompress);
            var reader = new StreamReader(responseStream);
            ResultString = reader.ReadToEnd().Replace("torrent:","torrent_");
            ResultXml= XDocument.Parse(ResultString);
            var result = ResultXml.Descendants("item");
            AllResults = result.Select(x => new TorrentDetails(x)).ToList();
            AllResultsView = CollectionViewSource.GetDefaultView(AllResults);
            Title = ResultXml.Descendants("channel").Descendants("title").FirstOrDefault().Value;
            Console.WriteLine(AllResults.Count);
        }

[AlsoNotifyFor("SelectionCount")]
        public ICollection<TorrentDetails> AllResults { get; set; }
        public ICollectionView AllResultsView { get; set; }
        public string Title { get; set; }

        public int SelectionCount
        {
            get
            {
                return AllResults.Count(x => x.Download);
            }
        }

        public TorrentDetails SelectedTorrent { get; set; }
    }
    [ImplementPropertyChanged]
    public class TorrentDetails
    {
        public TorrentDetails(XElement v)
        {
            Node = v;
            Category = v.Descendants("category").FirstOrDefault().Value;
            Enclosure = v.Descendants("enclosure").FirstOrDefault();

            if (Enclosure != null)
            {
                Url = Enclosure.Attribute("url").Value;
            }
            MagnetUri = v.Descendants("torrent_magnetURI").FirstOrDefault().Value;
            Title = v.Descendants("title").FirstOrDefault().Value;
            var r = v.Descendants("torrent_seeds").FirstOrDefault().Value; var q = 0; int.TryParse(r, out q); Seeds = q;
            r = v.Descendants("torrent_peers").FirstOrDefault().Value; q = 0; int.TryParse(r, out q); Peers = q;
            r = v.Descendants("torrent_verified").FirstOrDefault().Value; q = 0; int.TryParse(r, out q); Verified = q;
            Double t = 0; r = v.Descendants("torrent_contentLength").FirstOrDefault().Value; double.TryParse(r, out t); Length = t;

        }
        public XElement Node { get; set; }
        public XElement Enclosure { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string MagnetUri { get; set; }
        public int Seeds { get; set; }
        public int Peers { get; set; }
        public int Verified { get; set; }
        public string Title { get; set; }
        public double Length { get; set; }
        public bool Download { get; set; }


    }

    public class FileSizeToStringConverter : IValueConverter
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                var filesize = System.Convert.ToInt64(value);
                StringBuilder sb = new StringBuilder(11);
                StrFormatByteSize(filesize, sb, sb.Capacity);
                return sb.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
