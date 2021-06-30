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
using TvDbSearchControl;
using TvFileBot.ViewModel;

namespace TvFileBot
{
    /// <summary>
    /// Interaction logic for DirectoryManager.xaml
    /// </summary>
    public partial class DirectoryManager 
    {
        private DirectoryManagerVm _dc;

        public DirectoryManager()
        {
            InitializeComponent();
            _dc = DataContext as DirectoryManagerVm;
        }
        private async void MySearch_OnTap(object sender, RoutedEventArgs e)
        {
            var args = e as TvDataRoutedEventArgs;
            if (args == null) return;
            _dc.TvDbId = args.TvDbId.ToString();
            _dc.SelectedDirectoryShow.Save(_dc.TvDbId);
            _dc.IsSearching = false;
        }
    }
}
