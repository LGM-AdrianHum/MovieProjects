using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Movies {
    public class FileFolder {
        private readonly FileExtensions r = FileExtensions.Instance;

        public FileFolder(string s)
        {
            FullName = s;
            Name = Path.GetFileName(s);
            if (Directory.Exists(s)) Img = "folder";
            else
            {
                var extension = Path.GetExtension(s);

                if (!string.IsNullOrEmpty(extension)) Img = extension.Substring(1);

            }
        }

        public string FullName { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }

        public ImageSource GlowingImage
        {
            get
            {
                string fileName = "plain.ico";
                var j = r.ResourceList.FirstOrDefault(x => x.StartsWith(Img));
                if (!string.IsNullOrEmpty(j)) fileName = j;

                BitmapImage glowIcon = new BitmapImage();


                glowIcon.BeginInit();
                glowIcon.UriSource = new Uri("pack://application:,,,/images/" + fileName);
                glowIcon.EndInit();

                return glowIcon;
            }
        }
    }
}