using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using PropertyChanged;
using TVDBSharp;
using TVDBSharp.Models;
using TVDBSharp.Models.DAO;
namespace TvEpisodeNamer
{
    [ImplementPropertyChanged]
    public class TvEpisodesViewModel
    {
        private TvDb _tvdb;
        private BackgroundWorker _bgw;
        public const string WorkingDirectory = @"\\admin-pc\H\MyTv";
        public TvEpisodesViewModel()
        {
            //F2A9C6DEED8BBCD8
            _tvdb = new TVDBSharp.TvDb("F2A9C6DEED8BBCD8");
            RefreshDirectories();
            WorkQueue=new Queue<string>();
            _bgw=new BackgroundWorker();
            _bgw.RunWorkerCompleted += _bgw_RunWorkerCompleted;
            _bgw.DoWork += _bgw_DoWork;
            
            Doing = "Idle...";
        }

        private void _bgw_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        public int ProgressValue { get; set; }

        public string Doing { get; set; }

        void _bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Doing = "Idle...";
        }

        private void RefreshDirectories()
        {
            AllDirectories = Directory.GetDirectories(WorkingDirectory).Select(Path.GetFileName).ToList();
            AllDirectoriesView = CollectionViewSource.GetDefaultView(AllDirectories);
        }

        public ICollection<string> AllDirectories { get; set; }

        public string SelectedShowName { get; set; }

        public ICollectionView AllDirectoriesView
        {
            get;
            set;
        }

        public void SetWorkingShow(string text)
        {
            SelectedShowName = text;
            SelectedShowWorkingDirectory = Path.Combine(WorkingDirectory, text);
            var infoPath = Path.Combine(SelectedShowWorkingDirectory, "info.xml");
            SelectedShowDetails = Show.Load(infoPath);
            AllSeasons = (new[] { "All" }).ToList().Union(SelectedShowDetails.Episodes.Select(x => x.SeasonNumber)
                    .Distinct()
                    .Select(x => string.Format("S{0:00}", x))).ToList();
            AllEpisodes = SelectedShowDetails.Episodes.OrderBy(x => x.SeasonEpisode).ToList();
            AllEpisodesView = CollectionViewSource.GetDefaultView(AllEpisodes);
        }

        public void GetShowDetails()
        {
            SelectedShowDetails = _tvdb.GetShow(SelectedShowDetails.Id);
            var infoPath = Path.Combine(SelectedShowWorkingDirectory, "info.xml");
            SelectedShowDetails.Save(infoPath);
            SetWorkingShow(SelectedShowName);
        }

        public ICollectionView AllEpisodesView { get; set; }

        public ICollection<Episode> AllEpisodes { get; set; }


        public ICollection<string> AllSeasons { get; set; }

        public Show SelectedShowDetails { get; set; }

        public string SelectedShowWorkingDirectory { get; set; }

        public string SelectedSeason { get; set; }

        public void SetSeason(int i)
        {
            if (i == -1)
            {
                AllEpisodesView.Filter = null;
            }
            else
            {


                AllEpisodesView.Filter = item =>
                {
                    var vitem = item as Episode;
                    if (vitem == null) return false;

                    return vitem.SeasonNumber == i;

                };
            }
            AllEpisodesView.Refresh();
        }

        public void SearchNewShow(string text)
        {
            AllSearchResults = _tvdb.Search(text, 15);
            
        }

        public ICollection<Show> AllSearchResults { get; set; }

        public Show SelectedSearchShow
        {
            get;set;
        }

        public void CreateShow()
        {
            SelectedShowName = SelectedSearchShow.Name.ToSafeFilename();
            SelectedShowWorkingDirectory = Path.Combine(WorkingDirectory, SelectedShowName);
            var infoPath = Path.Combine(SelectedShowWorkingDirectory, "info.xml");
            
            
            AllSeasons = SelectedSearchShow.Episodes.Select(x => x.SeasonNumber)
                    .Distinct()
                    .Select(x => string.Format("S{0:00}", x)).ToList();
            foreach (var v in AllSeasons)
            {
                Directory.CreateDirectory(Path.Combine(SelectedShowWorkingDirectory, v));
            }
            AllSeasons = (new[] { "All" }).ToList().Union(SelectedSearchShow.Episodes.Select(x => x.SeasonNumber)
                    .Distinct()
                    .Select(x => string.Format("S{0:00}", x))).ToList();
            AllEpisodes = SelectedSearchShow.Episodes.OrderBy(x => x.SeasonEpisode).ToList();
            AllEpisodesView = CollectionViewSource.GetDefaultView(AllEpisodes);
            
            RefreshDirectories();

            SelectedShowDetails = SelectedSearchShow;
            SelectedSearchShow.Save(infoPath);

        }

        public void SetRenameTarget(string source="", Episode tar=null, bool commit=false)
        {
            if (tar != null) Target = tar;
            if (!string.IsNullOrEmpty(source)) Source = source;
            var ext = Path.GetExtension(Source);
            if (Target == null) return;
            TargetName = Path.Combine(SelectedShowWorkingDirectory, string.Format("S{0:00}", Target.SeasonNumber),
                string.Format("{0} - {1}", SelectedShowDetails.Name, Target.SeasonEpisodeTitle).ToSafeFilename()+ext
                );
            
            if (commit && !string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(TargetName))
            {
                var destination = new FileInfo(TargetName);
                var _source = new FileInfo(Source);
                if (destination.Exists)
                    destination.Delete();

                Task.Run(() =>
                {
                    _source.CopyTo(destination, x => ProgressValue = x);
                }).GetAwaiter().OnCompleted(() => MessageBox.Show("File Copied!"));
            }
        }

        public Queue<string> WorkQueue { get; set; }


        public string TargetName { get; set; }

        public string Source { get; set; }

        public Episode Target { get; set; }
    }

    public static class Helper
    {
        public static string ToSafeFilename(this string s)
        {
            return s.Replace(":", ",")
                .Replace("/", " ")
                .Replace("\\", " ")
                .Replace("*", " ")
                .Replace("?", ".")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", " ")
                .Replace("  ", " ");
        }
    }


    public static class FileInfoExtensions
    {
        public static void CopyTo(this FileInfo file, FileInfo destination, Action<int> progressCallback)
        {
            const int bufferSize = 1024 * 1024;  //1MB
            byte[] buffer = new byte[bufferSize], buffer2 = new byte[bufferSize];
            bool swap = false;
            int progress = 0, progress2 = 0, read = 0;
            long len = file.Length;
            float flen = len;
            Task writer = null;

            using (var source = file.OpenRead())
            using (var dest = destination.OpenWrite())
            {
                for (long size = 0; size < len; size += read)
                {
                    if ((progress = ((int)((size / flen) * 100))) != progress2)
                        progressCallback(progress2 = progress);
                    read = source.Read(swap ? buffer : buffer2, 0, bufferSize);
                    if (writer != null) writer.Wait();
                    writer = dest.WriteAsync(swap ? buffer : buffer2, 0, read);
                    swap = !swap;
                }
                dest.Write(swap ? buffer2 : buffer, 0, read);
            }
        }
    }
}
