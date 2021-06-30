using System;
using PropertyChanged;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMDbLib.Client;
using ValueScreen.Properties;

namespace ValueScreen
{
    [ImplementPropertyChanged]
    public class DirectoryHelper
    {
        public DirectoryHelper()
        {
            NavigateDownCommand = new RelayCommand(DoDoubleClick, o => true);
            FileRename = new RelayCommand(DoFileRename, o => true);
            MoveToFolder = new RelayCommand(DoMoveToFolder, o => true);
            NfoSaveCommand=new RelayCommand(DoNfoSaveCommand,
                o=>(SelectedMovie?.HasData() != null));
        }

        private void DoNfoSaveCommand(object obj)
        {
            if(SelectedMovie.HasData()) SelectedMovie.SaveBackData();
        }

        private void DoMoveToFolder(object obj)
        {
            if (string.IsNullOrEmpty(SelectedMovie.FileTitle)) return;
            var di = new DirectoryInfo(CurrentDirectory.Path);
            if (di.Parent == null) return;
            var k  = Path.Combine(di.Parent.FullName, SelectedMovie.FileTitle);
            if(string.Equals(k, di.FullName, StringComparison.CurrentCultureIgnoreCase)) di.MoveTo(k+"_");

            di.MoveTo(k);
            CurrentDirectory= new DirInfo(this,di.Parent);
            RefreshCurrentItems();

            var pt = CurrentDirectory.Root;
            if (pt == null) return;
            pt = System.IO.Path.Combine(pt, "movie.transfer");
            if (!Directory.Exists(pt)) Directory.CreateDirectory(pt);
            pt = System.IO.Path.Combine(pt, "MyMovies." + di.Name.Substring(0,1));
            if (!Directory.Exists(pt)) Directory.CreateDirectory(pt);
            pt = Path.Combine(pt, di.Name);

            if (!Directory.Exists(pt)) di.MoveTo(pt);
        }

        public void DoSearch()
        {
            var client = new TMDbClient("125e475654c781837d575da127e4d7bd");
            var results = client.SearchMovie(
                query:CurrentFilename.FileCleanNameNoYear,includeAdult:true);
            ListOfPossibleTitles = new List<TmDbCode>();

            if (results.Results == null) return;
            foreach (var result in results.Results)

                ListOfPossibleTitles.Add(
                    new TmDbCode()
                    {
                        Id = result.Id,
                        Name = result.Title,
                        Description = result.Overview,
                        Year = result.ReleaseDate?.Year.ToString() ?? ""
                    });
        }

        public IList<TmDbCode> ListOfPossibleTitles { get; set; }

        private void DoFileRename(object obj)
        {
            if (CurrentFilename == null) return;
            if (CurrentFilename.IsFile)
            {
                CurrentFilename.DoRename();
            }
            RefreshCurrentItems();
        }

        public string CleanDirectoryName { get; set; }

        private void DoDoubleClick(object obj)
        {
            var k = obj as DirInfo;
            if (k == null) return;

            if (k.IsDirectory)
            {
                CurrentDirectory = k;
                CurrentFilename = null;
                RefreshCurrentItems();
                return;
            }

            CurrentFilename = k;

            DoSearch();

            RefreshCurrentItems();
        }

        public TmDbCode SelectedMovie { get; set; }= new TmDbCode();

        public DirInfo CurrentFilename { get; set; }

        public List<DirInfo> HyperInlines { get; set; }

        public RelayCommand NavigateUp { get; set; }

        public RelayCommand FileRename { get; set; }
        public RelayCommand NavigateDownCommand { get; set; }
        public IList<DirInfo> CurrentItems { get; set; }

        public void RefreshCurrentItems()
        {
            IList<DirInfo> childDirList;

            //If current directory is "My computer" then get the all logical drives in the system
            if (CurrentDirectory.Name.Equals(Resources.My_Computer_String))
            {
                childDirList = (from rd in FileSystemExplorerService.GetRootDirectories()
                                select new DirInfo(this, rd)).ToList();
            }
            else
            {
                //Combine all the subdirectories and files of the current directory
                childDirList = (from dir in FileSystemExplorerService.GetChildDirectories(CurrentDirectory.Path)
                                select new DirInfo(this, dir)).ToList();

                IList<DirInfo> childFileList = (from fobj in FileSystemExplorerService.GetChildFiles(CurrentDirectory.Path)
                                                select new DirInfo(this, fobj)).ToList();

                childDirList = childDirList.Concat(childFileList).ToList();
            }

            CurrentItems = childDirList;
            if (CurrentDirectory.Path != null)
            {
                var m = new DirectoryInfo(CurrentDirectory.Path);
                if (m.Exists)
                {
                    var l = new List<DirInfo>();
                    while (m != null)
                    {
                        l.Add(new DirInfo(this, m));
                        m = m.Parent;
                    }

                    l.Reverse();
                    HyperInlines = l;
                }
            }
        }

        public DirInfo CurrentDirectory { get; set; }
        public object IsMovie { get; set; }
        public RelayCommand MoveToFolder { get; set; }

        public RelayCommand NfoSaveCommand { get; set; }
    }
}