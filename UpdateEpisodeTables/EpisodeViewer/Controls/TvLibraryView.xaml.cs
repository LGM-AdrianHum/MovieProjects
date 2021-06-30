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
using PropertyChanged;

namespace EpisodeViewer.Controls
{

    /// <summary>
    /// Interaction logic for TvLibraryView.xaml
    /// </summary>

    [ImplementPropertyChanged]
    public partial class TvLibraryView
    {
        public TvLibraryView()
        {
            InitializeComponent();
            
        }

        public TvLibraryData.TvLibraryItem SelectedTvDirectory
        {
            get { return (TvLibraryData.TvLibraryItem)GetValue(SelectedTvDir); }
            set { SetValue(SelectedTvDir, value); }
        }

        // Using a DependencyProperty as the backing store for MySelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTvDir =
            DependencyProperty.Register("SelectedTvDirectory", typeof(TvLibraryData.TvLibraryItem), typeof(TvLibraryView), new UIPropertyMetadata(null));

        private void ListBoxDirs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTvDirectory = ListBoxDirs.SelectedItem as TvLibraryData.TvLibraryItem;
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem != null)
            {
                listBox.Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        listBox.UpdateLayout();
                        if (listBox.SelectedItem !=
                            null)
                            listBox.ScrollIntoView(
                                listBox.SelectedItem);
                    }));
            }
        }
    }
}
