using System.IO;
using PropertyChanged;

namespace TvShowFileManager
{
    [ImplementPropertyChanged]
    public class DirInfo
    {
        public DirInfo(string s)
        {
            ShortName = Path.GetFileName(s);
            FullName = s;

        }

        public string FullName { get; set; }

        public string ShortName { get; set; }
        public string ShowIdFound { get; set; }

        public ShowData TvShowData { get; set; }

        public override string ToString()
        {
            return ShortName;
        }
    }
}