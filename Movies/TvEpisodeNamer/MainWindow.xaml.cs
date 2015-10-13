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
using TVDBSharp.Models;

namespace TvEpisodeNamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectedShow_OnGotFocus(object sender, RoutedEventArgs e)
        {
            Flyout.IsOpen = true;
            SelectedShow.Focus();
        }


        private void TvShowListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dc = DataContext as TvEpisodesViewModel;
            if (dc == null) return;
            SelectedShow.Text = (string) TvShowListBox.SelectedValue;
            dc.SetWorkingShow(SelectedShow.Text);
        }

        private void SeasonListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = DataContext as TvEpisodesViewModel;
            if (dc == null) return;
            var r = 0;
            var s = SeasonListBox.SelectedValue.ToString();
            if (s == "All") r = -1;
            else
            {
                int.TryParse(s.Substring(1), out r);
            }
            dc.SetSeason(r);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as TvEpisodesViewModel;
            
            var btn = sender as Button;
            if (btn == null || dc == null) return;
            switch (btn.CommandParameter.ToString())
            {
                case "Update":
                {
                    dc.GetShowDetails();
                    return;
                }
                case "Create":
                {
                    CreateShowFlyout.IsOpen = true;
                    return;
                }
            }
        }

        private void DirectorySelector_OnFileSelected(FileSelectedEventArgs args)
        {
            MoveTarget.Content = args.FileFolderName;
            var dc = DataContext as TvEpisodesViewModel;
            if (dc == null) return;
            dc.SetRenameTarget(args.FileFolderName);
        }

        private void EpisodeListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = DataContext as TvEpisodesViewModel;
            if (dc == null) return;
            var tar = EpisodeListBox.SelectedItem as Episode;
            dc.SetRenameTarget("",tar);
        }

        private void EpisodeListBox_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dc = DataContext as TvEpisodesViewModel;
            if (dc == null) return;
            var tar = EpisodeListBox.SelectedItem as Episode;
            dc.SetRenameTarget("", tar,true);

        }
    }
}
