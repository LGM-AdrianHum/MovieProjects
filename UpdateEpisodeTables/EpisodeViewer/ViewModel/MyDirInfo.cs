using System;
using System.IO;
using System.Linq;

namespace EpisodeViewer.ViewModel
{
    public class MyDirInfo
    {
        public MyDirInfo(DirectoryInfo di)
        {
            Di = di;
            HasData = di.GetFiles("tvdb_*.zip").Any();
            if (di.Exists)
                Directories = string.Join(", ", 
                    di.GetDirectories().Select(x => x.Name)
                        .Where(x=>!x.StartsWith("extra",StringComparison.CurrentCultureIgnoreCase)));
        }

        public string Directories { get; set; }

        public bool HasData { get; set; }

        public DirectoryInfo Di { get; set; }
        public string Name => Di.Name;

        
        public override string ToString()
        {
            return Name;
        }
    }
}