using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using EnterpriseDT.Net.Ftp;
using PropertyChanged;
using Relays;

namespace SyncDownload
{
    [ImplementPropertyChanged]
    public class SyncDownVm
    {
        public SyncDownVm()
        {
            BeginCatalog = new RelayCommand(DoBeginCatalog, o => true);
        }

        public int StatusCount { get; set; }

        public string StatusMessage { get; set; }

        public BackgroundWorker Bgw1 { get; set; }

        public RelayCommand BeginCatalog { get; set; }

        public static string FileRepository { get; set; } = @"K:\\BtSync\\tv";
        public static string HostName { get; set; } = "nqhf129.dediseedbox.com";
        public static string InitialDirectory { get; set; } = "/btsync/tv";
        public static string Password { get; set; } = "5ddu19c1t6al";
        public static string UserName { get; set; } = "lgm";

        private List<string> Messages { get; } = new List<string>();

        public static ObservableCollection<Infos> AllFiles { get; set; }

        public long Download1 { get; set; }
        public long Download2 { get; set; }
        public long Maximum1 { get; set; }
        public long Maximum2 { get; set; }

        public FTPConnection Ftp1 { get; set; }
        public FTPConnection Ftp2 { get; set; }

        private void DoBeginCatalog(object obj)
        {
            Bgw1 = new BackgroundWorker {WorkerReportsProgress = true};
            Bgw1.ProgressChanged += (ss, ee) =>
            {
                var rr = ee.UserState as BackgroundMessage;
                if (rr == null) return;
                StatusMessage = rr.Message;
                StatusCount = rr.Count;
            };
            Bgw1.DoWork += (ss, ee) => { ee.Result = DoRemoteScan(); };
            Bgw1.RunWorkerCompleted += Bgw1_RunWorkerCompleted;
            Bgw1.RunWorkerAsync();
        }

        private void Bgw1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                Console.WriteLine(e.Result.GetType());
                var qq = e.Result as List<Infos>;
                AllFiles = new ObservableCollection<Infos>();
                foreach (var q in qq)
                {
                    AllFiles.Add(q);
                }

                
                Console.WriteLine(AllFiles.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private List<Infos> DoRemoteScan(string host = "", string username = "", string password = "")
        {
            if (!string.IsNullOrEmpty(username)) UserName = username;
            if (!string.IsNullOrEmpty(host)) HostName = host;
            if (!string.IsNullOrEmpty(password)) Password = password;
            var ftp = new FTPConnection
            {
                ServerAddress = HostName,
                UserName = UserName,
                Password = Password
            };

            Ftp1= new FTPConnection
            {
                ServerAddress = HostName,
                UserName = UserName,
                Password = Password
            };

            Ftp2 = new FTPConnection
            {
                ServerAddress = HostName,
                UserName = UserName,
                Password = Password
            };


            ftp.ReplyReceived += Ftp_ReplyReceived;
            ftp.Connect();

            var directoryqueue = new List<string>();
            var files = new List<FTPFile>();
            directoryqueue.Add(InitialDirectory);
            var icnt = 0;
            while (directoryqueue.Any())
            {
                try
                {
                    var wd = directoryqueue.FirstOrDefault();
                    //if (wd.Contains("[") || wd.Contains("]"))
                    //{
                    //    //wd = wd.Replace("[", "%5B").Replace("]", "%5D");
                    //}
                    directoryqueue.RemoveAt(0);
                    ftp.ChangeWorkingDirectory(wd);
                    var brw = new BackgroundMessage();
                    foreach (var v in ftp.GetFileInfos())
                    {
                        icnt++;
                        Console.WriteLine(v.Path);
                        if (v.Dir)
                        {
                            brw.Title = v.Path;

                            directoryqueue.Add(v.Path);
                        }
                        else
                        {
                            var le = "k:" + v.Path.Replace("/", "\\");
                            files.Add(v);
                            brw.Message = v.Name;
                            brw.Count = files.Count + 1;
                            Bgw1.ReportProgress(icnt%100, brw);
                        }
                    }
                }
                catch (FTPException fex)
                {
                    Console.WriteLine("\r\nFTP Exception Encountered");
                    Console.WriteLine(fex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\r\nException Encountered");
                    Console.WriteLine(ex);
                }
                directoryqueue.Sort();
            }

            while (files.Any())
            {
                
            }


        }

        private void Ftp_ReplyReceived(object sender, FTPMessageEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { Messages.Add(e.Message); });
        }

        internal class BackgroundMessage
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public int Count { get; set; }
        }
    }

    public class Infos
        {
            public Infos(string x)
            {
                Name = x;
                //Size = x.Size;
                //Path = x.Path;
            }

            public string Path { get; set; }

            public long Size { get; set; }

            public string Name { get; set; }
        }
        
    
}