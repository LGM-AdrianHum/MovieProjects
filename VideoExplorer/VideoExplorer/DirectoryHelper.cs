using System.Collections.Generic;
using System.IO;
using System.Linq;
using PropertyChanged;
using VideoExplorer.Properties;

namespace VideoExplorer
{
    [ImplementPropertyChanged]
    public class DirectoryHelper
    {
        public ItemInfo CurrentDirectory { get; set; }
        public ItemInfo CurrentItem { get; set; }
        public DirectoryHelper(string s)
        {
            if (File.Exists(s))
            {
                CurrentItem=new ItemInfo(new FileInfo(s));
                CurrentDirectory = CurrentItem.Parent;
            }
        }

        public void Refresh()
        {
            IList<ItemInfo> childDirList;

            //If current directory is "My computer" then get the all logical drives in the system
            if (CurrentDirectory.Name.Equals(Resources.My_Computer_String))
            {
                childDirList = (from rd in FileSystemExplorerService.GetRootDirectories()
                                select new ItemInfo(rd)).ToList();
            }
            else
            {
                //Combine all the subdirectories and files of the current directory
                childDirList = (from dir in FileSystemExplorerService.GetChildDirectories(CurrentDirectory.Path)
                                select new ItemInfo(dir)).ToList();

                IList<ItemInfo> childFileList = (from fobj in FileSystemExplorerService.GetChildFiles(CurrentDirectory.Path)
                                                 select new ItemInfo(fobj)).ToList();

                childDirList = childDirList.Concat(childFileList).ToList();
            }

            CurrentItems = childDirList;
            if (CurrentDirectory.Path == null) return;
            var m = new DirectoryInfo(CurrentDirectory.Path);
            if (!m.Exists) return;
            var l = new List<ItemInfo>();
            while (m != null)
            {
                l.Add(new ItemInfo(m));
                m = m.Parent;
            }

            l.Reverse();
            HyperInlines = l;
        }

        public IList<ItemInfo> HyperInlines { get; set; }

        public IList<ItemInfo> CurrentItems { get; set; }
    }
}