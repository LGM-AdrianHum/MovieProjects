using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PropertyChanged;

namespace MediaFileBrowser.ViewModel
{
    [ImplementPropertyChanged]
    public class ItemBase
    {
        public ItemBase(string s, string d, int id )
        {
            Name = s;
            FullName = d;
            Id = id;
        }

        public int Id { get; set; }

        public ItemBase(string s, string typ = "")
        {
            FullName = s;
            IsMovie = s.IsMovie();
            Name = Path.GetFileName(s);
            if (typ != "parent")
            {
                typ = s.IsMovie() ? "movie" : "blank";
                if (Directory.Exists(s))
                {
                    typ = "folder";
                    var pthm1 = Path.Combine(s, "movie.nfo");


                    if (File.Exists(pthm1) || Directory.GetFiles(s, "*.tmdb").Any())
                    {
                        typ = "bluefolder";
                    }
                }

            }
            Ext = Path.GetExtension(s);
            
            if (Ext != null && Ext.ToLower() == ".nfo") typ = "nfo";
            if (Ext != null && Ext.ToLower() == ".png") typ = "images";
            if (Ext != null && Ext.ToLower() == ".jpg") typ = "images";
            if (Ext != null && Ext.ToLower() == ".tmdb") typ = "tvdb";
            if (Name != null && Name.ToLower() == "movie.nfo") typ = "tvdb";
            FileType = s.IsMovie().ToString();
            Console.WriteLine(s);
            switch (typ)
            {
                case "bluefolder":
                    {
                        ItemImageBase = SetImage("pack://application:,,,/Images/bluefolder.ico");
                        break;
                    }

                case "folder":
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/foldernotselected.ico");
                    break;
                }
                case "movie":
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/moviefile.ico");
                    break;
                }
                case "nfo":
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/nfo.ico");
                    break;
                }
                case "images":
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/images.ico");
                    break;
                }
                case "tvdb":
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/tvdb.ico");
                    break;
                }
                case "parent":
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/back.ico");
                    break;
                }
                case "blank":
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/blankfile.ico");
                    break;

                }


                default:
                {
                    ItemImageBase = SetImage("pack://application:,,,/Images/blankfile.ico");
                    break;

                }

            }

        }

        public bool IsMovie { get; set; }

        public string FileType { get; set; }

        public string Ext { get; set; }

        private static ImageSource SetImage(string s)
        {
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(s);
            logo.EndInit();
            return logo;
        }

        public ImageSource ItemImageBase { get; set; }

        public string FullName { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}