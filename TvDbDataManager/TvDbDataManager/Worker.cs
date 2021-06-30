using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using TVDB.Model;
using TVDB.Web;

namespace TvDbDataManager
{
    public class Worker
    {
        private WebInterface _client;
        public const string LanguageAbbreviation = "en";
        private const string CacheDirectory = @"K:\CacheData";
        public const string ApiKey = "F2A9C6DEED8BBCD8";

        public Mirror Mirror { get; set; }

        public void Init()
        {
            var wc = new WebClient();
            var str = $"http://thetvdb.com/api/{ApiKey}/mirrors.xml";
            var dt = wc.DownloadString(str);
            Console.WriteLine(dt);
            XElement xelement = XElement.Parse(dt);
            IEnumerable<XElement> mirrors = xelement.Elements();
            var firstOrDefault = mirrors.FirstOrDefault();
            var xElement = firstOrDefault?.Element("id");
            if (xElement != null)
            {
                MirrorId = xElement.Value;

            }
            xElement = firstOrDefault?.Element("mirrorpath");
            if (xElement != null)
            {
                MirrorPath = xElement.Value;

            }
            ProcessByName("Daredevil");

            InitializeDirectoryQueue();
        }

        public string MirrorPath { get; set; }

        public string MirrorId { get; set; }

        public List<string> DirectoryQueue { get; set; }

        public void InitializeDirectoryQueue()
        {
            if (DirectoryQueue == null) DirectoryQueue = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                var v = $"\\\\admin-pc.local\\MyTv{i.ToString("000")}";
                if (Directory.Exists(v))
                {
                    DirectoryQueue.AddRange(Directory.GetDirectories(v));
                }

                DirectoryQueue.RemoveAll(x =>
                {
                    var fileName = Path.GetFileName(x);
                    return fileName != null && fileName.StartsWith("$");
                });
            }

            Console.WriteLine("You have {0} directories to process.",DirectoryQueue.Count);

            Thread.Sleep(3000);
            var vdq = DirectoryQueue.FirstOrDefault();
            ProcessByName(vdq);
            
        }

        public void ProcessByName(string vdq)
        {
            var name = Path.GetFileName(vdq);

            var cnst = $"{MirrorPath}/api/GetSeries.php?seriesname={name}&language={LanguageAbbreviation}";
            var wc = new WebClient();
            var str = $"http://thetvdb.com/api/{ApiKey}/mirrors.xml";
            wc.DownloadStringCompleted += Wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri(cnst));

            
            while(wc.IsBusy) Thread.Sleep(1000);
            Thread.Sleep(6000);
            Console.WriteLine("Here");
            Console.ReadKey();
            
            
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var dt = e.Result;
            Console.WriteLine(dt);
            XElement xelement = XElement.Parse(dt);
            IEnumerable<XElement> datas = xelement.Elements();
            if (!datas.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There Was No Data");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            foreach (var dta in datas)
            {
                var l = new SeriesSearchInfo();

                var serializer = new XmlSerializer(typeof(SeriesSearchInfo));
                SeriesSearchInfo result;

                using (TextReader reader = new StringReader("<?xml version=\"1.0\" encoding=\"UTF - 8\" ?>" + dta.ToString()))
                {
                    result = (SeriesSearchInfo)serializer.Deserialize(reader);
                    result.GetData(this);
                }

                Console.WriteLine(result.Id);
            }
        }

        public string ResultBuffer { get; set; }

        public async void ProcessSingleSeries(int id)
        {



            // if (Mirror == null) return;
            // var result = await wc.DownloadDataTaskAsync(
            //$"{MirrorPath}/api/{ApiKey}/series/{id}/all/{LanguageAbbreviation}.zip").ConfigureAwait(continueOnCapturedContext: false);

            // var loadedSeriesPath = Path.Combine(CacheDirectory, "TvDb.ZipData", id.ToString("0000000000") + ".zip");

            // using (var zipFile = new FileStream(loadedSeriesPath, FileMode.Create, FileAccess.Write))
            // {
            //     zipFile.Write(result, 0, result.Length);
            //     zipFile.Flush();
            //     zipFile.Close();
            //}
        }
    }
    [Serializable()]
    [XmlRoot(ElementName = "Series")]
    public class SeriesSearchInfo
    {
        [XmlElement("seriesid")]
        public string seriesid;
        [XmlElement("language")]
        public string language;
        [XmlElement]
        public string SeriesName;
        [XmlElement("banner")]
        public string banner;
        [XmlElement]
        public string Overview;
        [XmlElement]
        public string FirstAired;
        [XmlElement]
        public string Network;
        [XmlElement("IMDB_ID")]
        public string IMDB_ID;
        [XmlElement("zap2it_id")]
        public string Zap2ItId;
        [XmlElement("id")]
        public int Id;

        public string CacheFileName()
        {
            return $"k:\\CacheData\\{Id.ToString("0000000000")} - {SeriesName}.zip";
        }

        public void GetData(Worker parent)
        {
            if (File.Exists(CacheFileName())) return;
            var url = $"{parent.MirrorPath}/api/{Worker.ApiKey}/series/{Id}/all/{Worker.LanguageAbbreviation}.zip";
            var wc=new WebClient();
            wc.DownloadFile(url,CacheFileName());
            Thread.Sleep(2000);
        }
    }
}