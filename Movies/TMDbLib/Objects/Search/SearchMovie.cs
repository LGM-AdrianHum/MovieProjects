using System;
using System.Collections.Generic;
using System.Linq;

namespace TMDbLib.Objects.Search
{
    public class SearchMovie
    {
        public int Id { get; set; }
        public bool Adult { get; set; }
        public string BackdropPath { get; set; }
        public string OriginalTitle { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string PosterPath { get; set; }
        public double Popularity { get; set; }
        public string Title { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public List<int> GenreIds { get; set; }
        public string Overview { get; set; }
        public string OriginalLanguage { get; set; }
        public bool Video { get; set; }

        public string MovieFormattedName
        {
            get
            {
                return ReleaseDate.HasValue
                    ? string.Format("{0} ({1:yyyy})", Title, ReleaseDate.Value)
                    : Title;
            }
        }

        
    }
}