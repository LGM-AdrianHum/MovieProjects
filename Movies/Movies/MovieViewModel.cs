// Author: Adrian Hum
// Project: Movies/MovieViewModel.cs
// 
// Created : 2015-10-02  12:27 
// Modified: 2015-10-04 21:22)

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using PropertyChanged;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace Movies {
    [ImplementPropertyChanged]
    public class MovieViewModel {
        private readonly TMDbClient _client;

        public MovieViewModel()
        {
            _client = new TMDbClient("125e475654c781837d575da127e4d7bd");
        }

        public ICollection<MovieFolder> AllMovieFolders { get; set; }
        public ICollectionView AllMovieFolderCollectionView { get; set; }

        public string Producer
        {
            get { return "C Citizen"; }
        }

        public string MovieCast
        {
            get { return "MovieCast"; }
        }

        public ObservableCollection<SearchMovie> AllSearchResults { get; set; }
        public bool IsBusy { get; set; }
        public string SearchMovieName { get; set; }
        public string SearchMovieYear { get; set; }
        public SearchMovie SelectedSearch { get; set; }

        public void SetMovieName(string s)
        {
            var regex = new Regex(@"^(?<MovieName>.+)\((?<Year>\d+)\)(?<AdditionalText>[^\.]*)\.(?<Extension>[^\.]*)$");
            var match = regex.Match(s);
            SearchMovieName = match.Groups["MovieName"].Value;
            SearchMovieYear = match.Groups["Year"].Value;
        }

        public async void GetMovies()
        {
            var n = SearchMovieName;
            var s = SearchMovieYear;
            var res =
                string.IsNullOrEmpty(s)
                    ? await _client.SearchMovie(n, 0, true, 0)
                    : await _client.SearchMovie(n, 0, true, int.Parse(s));
            AllSearchResults = new ObservableCollection<SearchMovie>(res.Results.ToList());
        }
    }
}