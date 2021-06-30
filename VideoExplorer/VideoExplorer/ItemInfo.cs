using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PropertyChanged;

namespace VideoExplorer
{
    [ImplementPropertyChanged]
    public class ItemInfo
    {
        public ItemInfo()
        {

        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        public ItemInfo(DirectoryInfo di)
        {
            ItemType = ItemTypeOf.Directory;
            FileInfo = null;
            DirInfo = di;
            Name = di.Name;
            Path = di.FullName;
            Parent = new ItemInfo(di.Parent);
        }

        public string Name { get; set; }

        public ItemInfo(FileInfo fi)
        {
            ItemType = ItemTypeOf.File;
            FileInfo = fi;
            DirInfo = fi.Directory;
            Name = fi.Name;
            Path = fi.FullName;
            Parent = new ItemInfo(DirInfo);
        }

        public ItemInfo(DriveInfo rd)
        {
            ItemType=ItemTypeOf.Drive;
            DirInfo = rd.RootDirectory;
            FileInfo = null;
            Name = rd.Name;
            Path = rd.RootDirectory.FullName;
        }

        public void ParseMovie()
        {
            const string p = ".avi|.mkv|.mp4";
            IsMovie = p.Contains(FileInfo.Extension.ToLower());

            if (!IsMovie) return;


            CleanWordAndNumbers = Regex.Replace(Name, @"[^\w\s]", " ");

            var rx = new Regex(@"\b(19|20)\d{2}\b");
            YearReleasedGuess = rx.Match(CleanWordAndNumbers).Value;
            var r = System.IO.Path.GetInvalidPathChars();
            var leftofyear =
                $"{Name.Substring(0, Name.IndexOf(YearReleasedGuess, StringComparison.Ordinal))}";

            leftofyear = leftofyear.Replace(".", " ").Trim().TrimEnd('(').Trim().TrimEnd('(').Trim();

            var cly = leftofyear.Replace(":", ",");
            cly = r.Aggregate(cly, (current, rr) => current.Replace(rr, ' '));

            var myTi = new CultureInfo("en-US", false).TextInfo;

            MovieName = Regex.Replace(myTi.ToTitleCase(cly), @"\s+", " ").Trim();

        }

        public string MovieName { get; set; }
        public bool IsMovie { get; set; }
        public string YearReleasedGuess { get; set; }
        public string CleanWordAndNumbers { get; set; }
        public DirectoryInfo DirInfo { get; set; }
        public FileInfo FileInfo { get; set; }
        public ItemTypeOf ItemType { get; set; }
        public string Path { get; set; }
        public ItemInfo Parent { get; set; }
    }
}