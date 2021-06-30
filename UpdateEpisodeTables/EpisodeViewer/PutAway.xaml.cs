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
using System.Windows.Shapes;

namespace EpisodeViewer
{
    /// <summary>
    /// Interaction logic for PutAway.xaml
    /// </summary>
    public partial class PutAway 
    {
        public PutAway()
        {
            
            InitializeComponent();
            
        }

        private void PutAway_Loaded(object sender, RoutedEventArgs e)
        {
            var pv = DataContext as PutAwayVm;
            LeftPane.DataContext = pv.FileVm;
            RightPane.DataContext = pv.TvVm;
            pv.FileVm.SelectionMade += FileVm_SelectionMade;
        }

        private void FileVm_SelectionMade(object sender, EventArgs e)
        {
            MessageBox.Show("Bang");
        }
    }
}
