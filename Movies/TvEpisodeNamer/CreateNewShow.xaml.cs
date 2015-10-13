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

namespace TvEpisodeNamer
{
    /// <summary>
    /// Interaction logic for CreateNewShow.xaml
    /// </summary>
    public partial class CreateNewShow : UserControl
    {
        public CreateNewShow()
        {
            InitializeComponent();
        }

        private void DoSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as TvEpisodesViewModel;
            if (dc == null) return;
            dc.SearchNewShow(MySearchValue.Text);
        }

        private void CreateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as TvEpisodesViewModel;
            if (dc == null) return;
            dc.CreateShow();
        }
    }
}
