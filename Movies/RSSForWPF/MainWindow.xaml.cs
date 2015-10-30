using PropertyChanged;
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
using System.Xml.Linq;

namespace WPFRSS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var cls = new RssFeedViewModel();
            DataContext = cls;
            cls.FeedFormat = "http://kat.cr/usearch/{0}/?rss=1";
            cls.Query = "complete category:tv";

        }

        private void HandleClick(object sender, RoutedEventArgs e)
        {
            var cls = DataContext as RssFeedViewModel;
            if (cls == null) return;
            cls.CleanupQuery();
            cls.DoQuery();






        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewSourceFlyout.IsOpen = !ViewSourceFlyout.IsOpen;
        }

        private void DataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dc = DataContext as RssFeedViewModel;
            if (dc == null || dc.SelectedTorrent == null) return;
            Clipboard.SetText(dc.SelectedTorrent.Url, TextDataFormat.Text);
        }
    }


}
