using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace TvShowFileManager
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

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var kb = sender as ListBox;
            if (kb == null) return;
            var lb = kb.SelectedItem;
            kb.ScrollIntoView(lb);
        }
    }
}
