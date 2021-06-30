using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace ValueScreen
{
    [ImplementPropertyChanged]
    public class DirInfo : DependencyObject
    {

        private void DoDeleteCommandExecute(object obj)
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }

            _directoryHelper?.RefreshCurrentItems();
        }
        public RelayCommand DoDeleteCommand { get; set; }

        public RelayCommand NavigateUpCommand { get; set; }
        private readonly DirectoryHelper _directoryHelper;
        public bool IsFile { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsMovie { get; set; }
        public VisualBrush IconType { get; set; }
        public string CleanWordAndNumbers { get; set; }
        public string FileCleanName { get; set; }
        public string FileFormattedName { get; set; }
        public string YearReleasedGuess { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
        public string Root { get; set; }
        public string Size { get; set; }
        public string Ext { get; set; }
        public int DirType { get; set; }

        public static readonly DependencyProperty PropertyChilds = DependencyProperty.Register("Childs",
            typeof(IList<DirInfo>), typeof(DirInfo));

        public IList<DirInfo> SubDirectories
        {
            get { return (IList<DirInfo>)GetValue(PropertyChilds); }
            set { SetValue(PropertyChilds, value); }
        }

        public static readonly DependencyProperty PropertyIsExpanded = DependencyProperty.Register("IsExpanded",
            typeof(bool), typeof(DirInfo));

        public bool IsExpanded
        {
            get { return (bool)GetValue(PropertyIsExpanded); }
            set { SetValue(PropertyIsExpanded, value); }
        }

        public static readonly DependencyProperty PropertyIsSelected = DependencyProperty.Register("IsSelected",
            typeof(bool), typeof(DirInfo));

        public bool IsSelected
        {
            get { return (bool)GetValue(PropertyIsSelected); }
            set { SetValue(PropertyIsSelected, value); }
        }

        public DirInfo()
        {
            SubDirectories = new List<DirInfo> { new DirInfo("TempDir") };
            DoDeleteCommand = new RelayCommand(DoDeleteCommandExecute, o => true);
        }

        public DirInfo(string directoryName)
        {
            Name = directoryName;
        }

        public DirInfo(DirectoryHelper t, DirectoryInfo dir)
            : this()
        {
            Name = dir.Name;
            Root = dir.Root.Name;
            Path = dir.FullName;
            DirType = (int)ObjectType.Directory;
            IsFile = false;
            IsDirectory = true;
            DoDeleteCommand = new RelayCommand(DoDeleteCommandExecute, o => true);
            NavigateUpCommand = new RelayCommand(DoNavigateUp, o => true);
            _directoryHelper = t;
        }
        public DirInfo(DirectoryHelper t, FileInfo fileobj)
        {
            Name = fileobj.Name;
            Path = fileobj.FullName;
            DirType = (int)ObjectType.File;
            Size = (fileobj.Length / 1024).ToString() + " KB";
            Ext = fileobj.Extension.ToLower();
            IsFile = true;
            IsDirectory = false;
            DoDeleteCommand = new RelayCommand(DoDeleteCommandExecute, o => true);

            CleanName();

            _directoryHelper = t;
        }

        private void DoNavigateUp(object obj)
        {
            if (_directoryHelper == null) return;
            _directoryHelper.CurrentDirectory = this;
            _directoryHelper.RefreshCurrentItems();
        }

  
        public DirInfo(DirectoryHelper t, DriveInfo driveobj)
            : this()
        {
            Name = driveobj.Name.EndsWith(@"\") ? driveobj.Name.Substring(0, driveobj.Name.Length - 1) : driveobj.Name;

            Path = driveobj.Name;
            DirType = (int)ObjectType.DiskDrive;
            _directoryHelper = t;
        }



        public void CleanName()
        {
            CleanWordAndNumbers = Regex.Replace(Name, @"[^\w\s]", " ");

            var rx = new Regex(@"\b(19|20)\d{2}\b");
            YearReleasedGuess = rx.Match(CleanWordAndNumbers).Value;
            var r = System.IO.Path.GetInvalidPathChars();
            var leftofyear =
                $"{Name.Substring(0, Name.IndexOf(YearReleasedGuess, StringComparison.Ordinal))}";

            leftofyear = leftofyear
            .Replace(".", " ")
                .Trim()
                .TrimEnd('(')
                                .Trim()
                .TrimEnd('(')

                .Trim();
            var cly = leftofyear.Replace(":", ",");
            cly = r.Aggregate(cly, (current, rr) => current.Replace(rr, ' '));
            var myTi = new CultureInfo("en-US", false).TextInfo;
            FileCleanNameNoYear =
                Regex.Replace(myTi.ToTitleCase(cly), @"\s+", " ").Trim();

            FileCleanName =
                Regex.Replace(FileCleanNameNoYear + $" ({YearReleasedGuess})", @"\s+", " ").Trim();
            FileFormattedName = FileCleanName + Ext.ToLower();

            const string p = ".avi|.mkv|.mp4";
            IsMovie = p.Contains(Ext.ToLower());
        }

        public string FileCleanNameNoYear { get; set; }

        public void DoRename()
        {
            var k = System.IO.Path.GetDirectoryName(Path);
            if (k == null) return;
            var cfs = System.IO.Path.Combine(k, FileFormattedName);
            File.Move(Path, cfs);
            Path = cfs;
        }
    }
}