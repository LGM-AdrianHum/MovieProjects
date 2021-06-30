using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using PropertyChanged;
using Relays;
using TVDB.Model;
using TVDB.Web;

namespace TvShowFileManager
{
    [ImplementPropertyChanged]
    public class MyManager
    {
    }

    [ImplementPropertyChanged]
    public class TvDirectories
    {
        public TvDirectories()
        {
            AllDirectories = new ObservableCollection<DirInfo>();
            ShowChoicesVisible = Visibility.Collapsed;
            RefreshDirectory = new RelayCommand(DoRefreshDirectories, o => true);
            SelectFromChoices = new RelayCommand(DoSelectFromChoices, o => ShowChoicesSelected != null);
            Message = "Ready...";

        }

        public RelayCommand RefreshDirectory { get; set; }

        public DirInfo ShowChoicesItem { get; set; }
        public DispatcherTimer WorkAlready { get; set; }
        public ListValues ShowChoicesSelected { get; set; }
        public Mirror Mirror { get; set; }
        public ObservableCollection<DirInfo> AllDirectories { get; set; }
        public ObservableCollection<ListValues> ShowChoices { get; set; }
        public Queue<DirInfo> WorkQueue { get; set; }
        public RelayCommand SelectFromChoices { get; set; }
        public string Message { get; set; }
        public string ShowChoicesTitle { get; set; }
        public Visibility ShowChoicesVisible { get; set; }
        public WebInterface Client { get; set; }

        private void DoSelectFromChoices(object obj)
        {
            ShowChoicesItem.TvShowData = new ShowData(ShowChoicesSelected.Id);
            ShowChoicesItem.ShowIdFound = "C:" + ShowChoicesSelected.Id;
            AllDirectories.Add(ShowChoicesItem);
            SelectedDirectory = ShowChoicesItem;
            WorkAlready.Start();
        }

        public DirInfo SelectedDirectory { get; set; }

        public void DoRefreshDirectories(object obj)
        {
            var a = Directory.GetDirectories(@"\\admin-pc.local\MyTv001").Union(
                Directory.GetDirectories(@"\\admin-pc.local\MyTv002"))
                .Select(x => new DirInfo(x));

            WorkQueue = new Queue<DirInfo>(a);
            TotalCount = WorkQueue.Count;
            MaxCount = WorkQueue.Count;
            StartProcessing();
        }

        public int MaxCount { get; set; }

        public int TotalCount { get; set; }

        public async void StartProcessing()
        {
            Client = new WebInterface("", @"k:\CacheData");
            Mirror = (await Client.GetMirrors()).FirstOrDefault();
            if (Mirror == null) return;


            WorkAlready = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3),
                IsEnabled = false
            };
            WorkAlready.Tick += WorkAlready_Tick;
            WorkAlready.Start();
        }

        private async void WorkAlready_Tick(object sender, EventArgs e)
        {
            WorkAlready.Stop();
            if (!WorkQueue.Any())
            {
                WorkAlready = null;
            }
            TotalCount = WorkQueue.Count;
            var dirInfos = WorkQueue.Dequeue();
            var a = await Client.GetSeriesByName(dirInfos.ShortName, Mirror);
            if (a.Count == 1)
            {
                var bb = a.FirstOrDefault();
                if (bb == null) return;
                var b = new ShowData(a.FirstOrDefault());
                dirInfos.TvShowData = b;
                dirInfos.ShowIdFound = "A:" + b.ShowDataId;
                SelectedDirectory = dirInfos;
                AllDirectories.Add(dirInfos);
                ShowChoicesVisible = Visibility.Collapsed;
                WorkAlready.Start();
                return;
            }
            else if (a.Count > 1)
            {
                ShowChoices = new ObservableCollection<ListValues>();
                foreach (var aa in a)
                {
                    var sn = $"{aa.Name} {aa.FirstAired.Year} {aa.Status}";
                    ShowChoices.Add(new ListValues { Id = aa.SeriesId, Name = sn });
                }
                ShowChoicesSelected = ShowChoices.Any() ? ShowChoices.FirstOrDefault() : null;
                ShowChoicesVisible = Visibility.Visible;
                ShowChoicesTitle = dirInfos.ShortName;
                ShowChoicesItem = dirInfos;
            }
        }
    }


}