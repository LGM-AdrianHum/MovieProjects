using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using MahApps.Metro.Controls;

namespace MovieNamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var dc = DataContext as TmDbViewModel;
            if (dc == null) return;

        }

        private async void SearchTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dc = DataContext as TmDbViewModel;
            if (dc == null) return;

            if (e.Key == Key.Enter)
            {
                await dc.GetMovies();
            }
            if (e.Key == Key.Down && SearchResultsListBox.SelectedIndex < dc.AllSearchResults.Count)
            {
                SearchResultsListBox.SelectedIndex++;
            }
            if (e.Key == Key.Up && SearchResultsListBox.SelectedIndex > 0)
            {
                SearchResultsListBox.SelectedIndex--;
            }

        }

        private void SearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var rx = new Regex("\\d{4}");
            var b = SearchTextBox.Text;
            var match = rx.Match(b);
            if (!match.Success) return;
            SearchYearTextBox.Text = match.Value;
            SearchTextBox.Text = b.Substring(0, match.Index).Trim();
            SearchTextBox.SelectionStart = SearchTextBox.Text.Length;

        }

        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            FrameworkElement element = textBlock.Parent as FrameworkElement;
            //textBlock.Margin = new Thickness((element.ActualWidth / 100) * 40, 0, 0, 0);
            textBlock.MinWidth = element.ActualWidth / 100 * 60;
            textBlock.TextAlignment = TextAlignment.Right;

        }


        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                HandleFileOpen(files[0]);
            }
        }

        public string DroppedFilename { get; set; }

        private void HandleFileOpen(string s)
        {
            DroppedFilename = s;
            var ss = Regex.Replace(System.IO.Path.GetFileNameWithoutExtension(s), "[^0-9a-zA-Z ]", " ");

            SearchTextBox.Text = ss;
            SearchTextBox.Focus();
            var dc = DataContext as TmDbViewModel;
            if (dc == null) return;
            dc.DropFileName = s;
            dc.DropNameOnly = System.IO.Path.GetFileName(s);
            dc.DropFileExtension = System.IO.Path.GetExtension(s);
            dc.SetTargetFilename();

        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = DataContext as TmDbViewModel;
            if (dc == null) return;
            dc.GetMovieDetails();

        }

        private void RenameAction_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as TmDbViewModel;
            if (dc == null) return;
            dc.RenameFile();
        }
    }
}
