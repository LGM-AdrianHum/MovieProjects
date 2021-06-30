using System;
using System.IO;
using EnterpriseDT.Net.Ftp;

namespace SyncFtpConsole
{
    public class DirInfo
    {
        public DirInfo()
        {
        }

        public DirInfo(string vv)
        {
            var v = new FileInfo(vv);
            FullPath = v.FullName;
            FileDate = v.LastWriteTime;
            Extension = v.Extension;
            Size = v.Length;
            Clean();
            v = null;
        }

        public DirInfo(FTPFile v)
        {
            FullPath = v.Path;
            FileDate = v.LastModified;
            Size = v.Size;
            Clean();
        }

        public string Key { get; set; }
        public string Extension { get; set; }
        public DateTime FileDate { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }

        public void Clean()
        {
            Key = FullPath;
            if (Key.Substring(1, 2) == ":\\") Key = FullPath.Substring(2);
            Key = Key.Replace("\\", "/").ToLower();
        }
    }
}