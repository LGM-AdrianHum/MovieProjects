using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PropertyChanged;

namespace MovieCommunicator
{
    [ImplementPropertyChanged]
    public class MovieViewModel
    {
        public ObservableCollection<MovieItem> SelectionOfMovies { get; set; }

        public MovieViewModel()
        {
            var mylist = Directory.GetDirectories(@"n:\MyMovies.Series\Series.AirportCollection\").Select(x=>new MovieItem(x));
            SelectionOfMovies=new ObservableCollection<MovieItem>(mylist);
        }
    }

    [ImplementPropertyChanged]
    public class MovieItem
    {
        

        public MovieItem(string x)
        {
            DirectoryName = x;
            Name = Path.GetFileName(x);
            var rx=new Regex("\\(\\d{4}\\)");
            ReleaseYear = rx.Match(Name).Value;
            PosterPath = Path.Combine(x, "poster.jpg");
            if (!File.Exists(PosterPath)) PosterPath = "";
        }

        public string PosterPath { get; set; }

        public string DirectoryName { get; set; }

        public string Name { get; set; }
        public string ReleaseYear { get; set; }
    }
}
