using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EnterpriseDT.Net.Ftp;

namespace SyncDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private long _filesize;
        //private FTPConnection ftp;
        //private bool _cancelling;

        public MainWindow()
        {
            InitializeComponent();
        }
        //public static string FileRepository { get; set; } = @"K:\\BtSync";
        //public static string HostName { get; set; } = "nqhf129.dediseedbox.com";
        //public static string InitialDirectory { get; set; } = "/btsync";
        //public static string Password { get; set; } = "5ddu19c1t6al";
        //public static string UserName { get; set; } = "lgm";
        //private void Button_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (ftp != null && ftp.IsTransferring)
        //    {
        //        _cancelling = true;
        //        return;
        //    }

        //    var bgw = new BackgroundWorker();
        //    bgw.DoWork += (ss, ee) =>
        //    {
        //        ftp = new FTPConnection
        //        {
        //            ServerAddress = HostName,
        //            UserName = UserName,
        //            Password = Password
        //        };
        //        ftp.ReplyReceived += Ftp_ReplyReceived;
        //        ftp.Downloading += Ftp_Downloading;
        //        ftp.BytesTransferred += Ftp_BytesTransferred;
        //        ftp.Downloaded += Ftp_Downloaded;
        //        ftp.Connect();
        //        var a1 = @"/btsync/movie/The.Return.Of.Godzilla.1984.iNTERNAL.BDRip.x264-PAST/The.Return.Of.Godzilla.1984.BDRip.x264-PAST.mkv";
        //        var b1 = "k:" + a1.Replace("/", "\\");

        //        var fi = new FileInfo(b1);
        //        if (!Directory.Exists(fi.DirectoryName)) Directory.CreateDirectory(fi.DirectoryName);
        //        ftp.TransferType = FTPTransferType.BINARY;
        //        if (fi.Exists) ftp.ResumeNextTransfer();
        //        ftp.DownloadFile(b1, a1);
        //    };
        //    bgw.RunWorkerAsync();
        //}

        //private void Ftp_Downloaded(object sender, FTPFileTransferEventArgs e)
        //{
        //    MessageBox.Show("Downloaded");
        //}

        //private void Ftp_BytesTransferred(object sender, BytesTransferredEventArgs e)
        //{
        //    if (_cancelling)
        //    {
        //        _cancelling = false;
        //        ftp.CancelTransfer();
        //    }

        //    App.Current.Dispatcher.Invoke(new Action(() =>
        //    {
        //        Bar.Value = e.ResumeOffset + e.ByteCount;
        //        /* Your code here */
        //        //if (e.ByteCount > 10e6) ftp.CancelTransfer();
        //        Lab.Text = ($"{e.ByteCount}\r\n" +
        //        $"{ e.ResumeOffset + e.ByteCount}" +
        //        $"\r\n{_filesize}");
        //    }));

        //}

        //private void Ftp_Downloading(object sender, FTPFileTransferEventArgs e)
        //{
        //    App.Current.Dispatcher.Invoke(new Action(() =>
        //        { 
        //            Bar.Minimum = 0;
        //            Bar.Maximum = e.RemoteFileSize;

        //            Lab2.Text = e.LocalFile;
        //            _filesize = e.RemoteFileSize;
        //        }));
        //}

        //private void Ftp_ReplyReceived(object sender, FTPMessageEventArgs e)
        //{
        //   // Console.WriteLine(e.Message);
        //}
    }
}
