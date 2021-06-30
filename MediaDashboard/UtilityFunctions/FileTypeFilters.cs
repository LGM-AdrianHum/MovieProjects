using System.IO;
using System.Linq;

namespace UtilityFunctions
{
    public static class FileTypeFilters
    {
        public static bool IsMovie(this string data)
        {
            var allvideotype = ".avi|.iso|.m4v|.mkv|.mp4|.mpg|.vob".Split('|');
            if (allvideotype.Contains(data.ToLower())) return true; // extension only handler.
            var ext = Path.GetExtension(data);
            return allvideotype.Contains(ext.ToLower());
        }

        public static bool IsMovie(this FileInfo data)
        {
            return data != null && data.Extension.IsMovie();
        }


        public static bool IsBitmap(this string data)
        {
            var allpicturetypes =".png|.jpg|.jpeg|.bmp|.gif".Split('|');
            if (allpicturetypes.Contains(data.ToLower())) return true; // extension only handler.
            var ext = Path.GetExtension(data);
            return allpicturetypes.Contains(ext.ToLower());
        }
        public static bool IsBitmap(this FileInfo data)
        {
            return data != null && data.Extension.IsBitmap();
        }
    }
}