using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFunctions.FolderHelper
{
    public static class FilterFiles
    {
        public static List<FileInfo> GetVideoFile(string wd)
        {
            var di = new DirectoryInfo(wd);
            return di.GetFiles("*.*", SearchOption.AllDirectories).AsParallel().Where(x => x.IsMovie()).ToList();
        }

        public static List<FileInfo> GetImageFile(string wd)
        {
            var di = new DirectoryInfo(wd);
            return di.GetFiles("*.*", SearchOption.AllDirectories).AsParallel().Where(x => x.IsBitmap()).ToList();
        }

        public static List<SmallInfo> AsSmallInfos(this IEnumerable<FileInfo> k)
        {
            return k.AsParallel().Select(x => new SmallInfo(x)).ToList();
        }
        public class SmallInfo
        {
            public SmallInfo(FileInfo f)
            {
                MyFileInfo = f;
            }

            public FileInfo MyFileInfo { get; set; }
            public string FullName => MyFileInfo.FullName;
            public string Name => MyFileInfo.Name;
            public long Size => MyFileInfo.Length;
            public string SizeP => MyFileInfo.Length.ToBytes();
            public DateTime Creation => MyFileInfo.CreationTime;
            public string Extension => MyFileInfo.Extension.ToLower();
        }
    }
}

