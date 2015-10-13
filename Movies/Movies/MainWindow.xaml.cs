using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Converters;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TMDbLib.Objects.Search;
using Path = System.IO.Path;

namespace Movies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            WindowState = WindowState.Maximized;
            InitializeComponent();
            SearchMoviePane.SearchResultBox.PreviewMouseDown += SearchResultBox_PreviewMouseDown;
            SearchMoviePane.SearchResultBox.PreviewKeyDown += SearchResultBox_PreviewKeyDown;
        }

        void SearchResultBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DisplayMovieDetails(SearchMoviePane.SearchResultBox);
        }
        void SearchResultBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DisplayMovieDetails(SearchMoviePane.SearchResultBox);
        }

        private async void DisplayMovieDetails(ListBox searchResultBox)
        {
            var selitem = searchResultBox.SelectedItem as SearchMovie;
            if (selitem == null) return;
            var dc = DataContext as SearcherViewModel;
            if (dc == null) return;

            var oldname = dc.OperationsFilePath;
            var oldpath = Path.GetDirectoryName(oldname);
            if (!File.Exists(oldname)) return;

            var fi = new FileInfo(oldname);
            var di = fi.Directory;
            if (di == null) return;
            var title = selitem.Title.ToSafeFilename();
            if (selitem.ReleaseDate.HasValue)
                title = string.Format("{0} ({1:yyyy})", selitem.Title, selitem.ReleaseDate.Value).ToSafeFilename();
            var diroot = di.Root.ToString();
            if (!diroot.EndsWith("\\")) diroot += "\\";
            var watchdir = string.Format("{2}_watch\\MyMovies.{0}\\{1}", title.Substring(0, 1), title, diroot);
            var newfile = title + Path.GetExtension(oldname);
            var newfullpath = Path.Combine(watchdir, newfile);


            var res = await this.ShowMessageAsync(Title,
                string.Format("Rename:\r\n\t{0}\r\nas\r\n\t{1}", oldname, newfullpath),
                MessageDialogStyle.AffirmativeAndNegative);
            if (res == MessageDialogResult.Affirmative)
            {
                Directory.CreateDirectory(watchdir);
                File.Move(oldname, newfullpath);

                DirectorySelector.DirectoryBox.Text = oldpath.Substring(0, oldpath.LastIndexOf("\\", StringComparison.Ordinal));
                DirectorySelector.SetWorkingDirectory();
                Thread.Sleep(300);

                DirectorySelector.DirectoryBox.Text += "\\";
                DirectorySelector.SetWorkingDirectory();
            }
        }





        private void DirectorySelector_OnFileSelected(FileSelectedEventArgs args)
        {
            var dc = DataContext as SearcherViewModel;
            if (dc == null) return;
            dc.OperationsFilePath = args.FileFolderName;
        }

        private void MovSrch_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = SearchMoviePane.DataContext as MovieViewModel;
            if (dc == null) return;
            dc.SetMovieName(OperationsFileTextBox.Text);
            SearchMovieFlyout.IsOpen = true;
        }

    }
}