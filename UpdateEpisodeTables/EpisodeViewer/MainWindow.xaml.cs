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

namespace EpisodeViewer
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

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Remove(DirectoryListGrid);
            LeftFlyout.Content = DirectoryListGrid;
            LeftFlyout.IsOpen = true;
            MainGrid.Children.Remove(ShowDetailsGrid);
            LeftTab.Content = ShowDetailsGrid;
            MainGrid.Children.Remove(FileListBox);
            RightTab.Content = FileListBox;
            MainGrid.Visibility=Visibility.Collapsed;
            MainTab.Visibility=Visibility.Visible;
        }
    }
}
