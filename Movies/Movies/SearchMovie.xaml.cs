using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Movies
{
    /// <summary>
    /// Interaction logic for SearchMovie.xaml
    /// </summary>
    public partial class SearchMoviePane
    {
        private MovieViewModel _dc;

        public SearchMoviePane()
        {
            InitializeComponent();
            _dc = new MovieViewModel();
        }

        private async void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            var v = DataContext as MovieViewModel;
            v.SearchMovieName = MovieName.Text;
            v.SearchMovieYear = MovieYear.Text;
            v.GetMovies();
        }

        
    }
}
