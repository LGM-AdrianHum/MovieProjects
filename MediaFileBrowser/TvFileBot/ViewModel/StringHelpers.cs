using System.IO;
using System.Linq;

namespace TvFileBot.ViewModel
{
    public static class StringHelpers
    {
        public static bool IsMovie(this string data)
        {
            var allvideotype = ".avi|.iso|.m4v|.mkv|.mp4|.mpg|.vob".Split('|');
            if (allvideotype.Contains(data.ToLower())) return true; // extension only handler.
            var ext = Path.GetExtension(data);
            return allvideotype.Contains(ext.ToLower());
        }
    }
}