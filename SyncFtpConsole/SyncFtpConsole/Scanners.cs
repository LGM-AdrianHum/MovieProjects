using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnterpriseDT.Net.Ftp;
using NLog;
using Logger = NLog.Logger;

namespace SyncFtpConsole
{
    public static class Scanners
    {
        private static Logger _logger = LogManager.GetLogger("SyncFTPConsole");
        public static Dictionary<string, DirInfo> RemoteFiles { get; set; }
        public static string FileRepository { get; set; } = @"K:\\BtSync";
        public static string HostName { get; set; } = "nqhf129.dediseedbox.com";
        public static string InitialDirectory { get; set; } = "/btsync";
        public static string Password { get; set; } = "5ddu19c1t6al";
        public static string UserName { get; set; } = "lgm";

        private static void DoLocalScan()
        {
            if (!Directory.Exists(FileRepository)) return;
            var di = new List<string> {FileRepository};
            //var files = new List<DirInfo>();
            while (di.Any())
            {
                var dd = di.FirstOrDefault();
                if (string.IsNullOrEmpty(dd) || !Directory.Exists(dd)) continue;
                di.Remove(dd);
                var d = Directory.GetDirectories(dd);
                di.AddRange(d);
                di.Sort();
                foreach (var f in Directory.GetFiles(dd))
                {
                    var k = new DirInfo(f);
                    if (!RemoteFiles.ContainsKey(k.Key))
                    {
                        ProcessLocalFile(k);
                    }
                }
                var dc = dd;
                while (!string.IsNullOrEmpty(dc))
                {
                    if (!(Directory.GetFiles(dc).Any() || Directory.GetDirectories(dc).Any()))
                    {
                        Directory.Delete(dc);
                    }
                    dc = Directory.GetParent(dc)?.FullName;
                }
            }
        }

        private static void ProcessLocalFile(DirInfo d)
        {
            var flagdelete = false;
            var f = Path.GetFileName(d.FullPath);
            if (f == null || !File.Exists(d.FullPath)) return;
            if ("rarbg.com" + d.Extension == f.ToLower()) flagdelete = true;
            if (f.StartsWith("downloaded from", StringComparison.InvariantCultureIgnoreCase)) flagdelete = true;
            if (d.Extension.Equals(".torrent", StringComparison.InvariantCultureIgnoreCase)) flagdelete = true;

            if (flagdelete)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\"{f}\" Deleted.");
                _logger.Warn($"\"{f}\" Deleted.");
                File.Delete(d.FullPath);
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            var k = d.FullPath.Split('\\');
            if (k.Length <= 2 || k[1].ToLower() != "btsync") return;
            k[1] = "BTSync.Archive";
            var newname = string.Join("\\", k);
            var p = Path.GetDirectoryName(newname);
            if (p != null && !Directory.Exists(p)) Directory.CreateDirectory(p);
            if (p == null || !Directory.Exists(p)) return;
            if (File.Exists(newname)) File.Delete(newname);
            try
            {

                File.Move(d.FullPath, newname);
                Console.WriteLine($"\"{f}\" moved.");
                _logger.Warn($"\"{f}\" moved.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor=ConsoleColor.Red;
                _logger.Error(ex, $"\"{f}\" could not be moved.");
                Console.WriteLine($"\r\n\"{f}\" could not be moved.\r\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            d.FullPath = newname;
        }

        private static void DoRemoteScan(string host = "", string username = "", string password = "")
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
            ftp.ReplyReceived += Ftp_ReplyReceived;
            ftp.Connect();

            var directoryqueue = new List<string>();
            var files = new List<DirInfo>();
            directoryqueue.Add(InitialDirectory);
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

                    foreach (var v in ftp.GetFileInfos())
                    {
                        if (v.Dir)
                        {
                            directoryqueue.Add(v.Path);
                        }
                        else
                        {
                            files.Add(new DirInfo(v));
                            var cc = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("\t" + v.Name);
                            _logger.Info($"\t{v.Name}");
                            Console.ForegroundColor = cc;
                        }
                    }
                }
                catch (FTPException fex)
                {
                    _logger.Error("FTP Error Encountered "+ fex);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\r\nFTP Exception Encountered");
                    Console.WriteLine(fex);
                    
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\r\nException Encountered");
                    Console.WriteLine(ex);
                    
                }
                directoryqueue.Sort();
            }

            var sl = files.Select(v => $"{v.FullPath}|{v.Size}|{v.FileDate}").ToList();
            sl.Sort();
            //var k = string.Join("\r\n", sl.ToArray());
            RemoteFiles = files.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y);
        }

        private static void Ftp_ReplyReceived(object sender, FTPMessageEventArgs e)
        {
            _logger.Debug(e.Message);

            var l = e.Message.Substring(0, 3);
            if (l.StartsWith("2")) Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(e.Message.Substring(0, 3));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(e.Message.Substring(3));
        }

        public static void Begin()
        {
            try
            {
                Console.Write("Process Will Begin In 3 seconds...");
                Reader.ReadLine(3000);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("   Ok.");
            }
            DoRemoteScan();
            DoLocalScan();
           
        }
    }
}