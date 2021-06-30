using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ReadFavoritesFromTorrentSites.Annotations;
using Relays;

namespace ReadFavoritesFromTorrentSites
{
    public class MainVm : INotifyPropertyChanged
    {
        public MainVm()
        {
            IoOperations = new Relays.RelayCommand(DoIoOperations, o => true);
        }

        private void DoIoOperations(object obj)
        {
            var o = obj.ToString();
            switch (o)
            {
                case "open":
                    {
                        var tx = File.ReadAllText(@"c:\1\ThePirateBay.html");
                        EditText = tx;
                        OnPropertyChanged(nameof(EditText));
                        break;
                    }
                case "get":
                {
                    AllNodes = new ObservableCollection<CategoryNode>();
                    var dt = ProcessPages(Category.AudioBooks);
                    AllNodes.Add(new CategoryNode
                    {
                        Name="AudioBooks",
                        Nodes = dt,
                    });
                    dt = ProcessPages(Category.TvShows);
                    AllNodes.Add(new CategoryNode
                    {
                        Name = "TvShows",
                        Nodes = dt,
                    });
                    OnPropertyChanged(nameof(AllNodes));
                        break;
            }
                case "save":
                    {
                        ProcessData(EditText);
                        break;
                    }
            }
        }

        public enum Category
        {
            AudioBooks = 102,
            TvShows = 205
        }

        private List<TorrentNode> ProcessPages(Category ct)
        {
            //https://thepiratebay.org/browse/102/2/3
            //
            var wc = new WebClient();
            var ctdata = new List<TorrentNode>();
            for (var i = 1; i < 11; i++)
            {

                wc.Headers.Add("User-Agent",
                    @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36");
                var tx = wc.DownloadString(new Uri($"https://thepiratebay.org/browse/{(int)ct}/{i}/3"));
                EditText = tx;
                ctdata.AddRange(ProcessData(tx));
            }

            return ctdata;
            
        }

        private List<TorrentNode> ProcessData(string tx)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(tx);
            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='detName']");
            var ll = new List<TorrentNode>();
            foreach (var k in htmlNodes)
            {
                var t = k.SelectSingleNode("a").InnerText;
                var m = k.ParentNode.SelectSingleNode("a[@title]")?.Attributes.FirstOrDefault(x => x.Name == "href")?.Value;
                var n = new TorrentNode
                {
                    Title = t.Trim(),
                    Magnet = m
                };
                ll.Add(n);
            }

            return ll;
        }

        public ObservableCollection<CategoryNode> AllNodes { get; set; }

        public string EditText { get; set; }

        public RelayCommand IoOperations { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CategoryNode {
        public string Name { get; set; }
        public List<TorrentNode> Nodes { get; set; }
    }

    public class TorrentNode
    {
        public string Title { get; set; }
        public string Magnet { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Title;
        }
    }
}
