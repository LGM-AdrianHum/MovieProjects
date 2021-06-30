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
using EpisodeViewer.Controls;
using PropertyChanged;

namespace EpisodeViewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class Window1 : Window
    {
        private TvLibraryData.TvLibraryItem _myDir;

        public Window1()
        {
            InitializeComponent();
            SelDir=new TvLibraryData();
            SelDir.SelectionMade += SelDir_SelectionMade;
        }

        private void SelDir_SelectionMade(object sender, EventArgs e)
        {
            MessageBox.Show(SelDir.SelectedDirectory.Name);
        }

        public TvLibraryData.TvLibraryItem MyDir
        {
            get { return _myDir; }
            set
            {
                _myDir = value;
                MessageBox.Show(value.FullName);
            }
        }

        public TvLibraryData SelDir { get; set; }
    }
}
