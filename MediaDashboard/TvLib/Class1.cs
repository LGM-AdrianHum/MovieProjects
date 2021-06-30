using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PropertyChanged;
using Relays;
using TvLib.Properties;
using TVDB.Model;
using TVDB.Utility;
using TVDB.Web;

namespace TvLib
{
    [ImplementPropertyChanged]
    public class ViewModel
    {
        private string _searchfilter;

        public ViewModel()
        {
            var r = new DirectoryInfo(Settings.Default.LibraryPath);
            if (r.Exists)
            {
                AllDirectories = new ObservableCollection<DirectoryData>(
                    FileNameHelpers.GetDirectories(r).Select(x => new DirectoryData(x)).OrderBy(x => x.LibraryDirectory.Name));
            }
            else AllDirectories = new ObservableCollection<DirectoryData>();
            CollViewSource = new CollectionViewSource {Source = AllDirectories}; //onload of your VM class
            //after ini YourCollection
            ClearFilter=new RelayCommand(DoClearFilter, o=>true);
            SelectShow=new RelayCommand(DoSelectShow,o=>true);

            DgVisible = true;
        }

        private void DoSelectShow(object obj)
        {
            DgVisible = !DgVisible;
            ShowDetailsVisible = !DgVisible;
        }

        public bool ShowDetailsVisible { get; set; }

        public bool DgVisible { get; set; }

        public RelayCommand SelectShow { get; set; }

        private void DoClearFilter(object obj)
        {
            SearchFilter = "";
        }

        public RelayCommand ClearFilter { get; set; }

        public ObservableCollection<DirectoryData> AllDirectories { get; set; }

        public CollectionViewSource CollViewSource { get; set; }
        public string SearchFilter
        {
            get { return _searchfilter; }
            set
            {
                _searchfilter = value;
                if (!string.IsNullOrEmpty(SearchFilter))
                    AddFilter();

                CollViewSource.View.Refresh(); // important to refresh your View
            }
        }
        private void AddFilter()
        {
            CollViewSource.Filter -= new FilterEventHandler(Filter);
            CollViewSource.Filter += new FilterEventHandler(Filter);

        }

        private void Filter(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as DirectoryData;
            if (src == null)
                e.Accepted = false;
            else if (src.LibraryDirectory?.Name != null && !src.LibraryDirectory.Name.StartsWith(SearchFilter, StringComparison.CurrentCultureIgnoreCase))
                e.Accepted = false;
        }

    }

    public class DirectoryData
    {
        public DirectoryData(DirectoryInfo d)
        {
            LibraryDirectory = d;
            ShowId = WebInterface.GetShowId(d);
           // ShowData = WebInterface.GetShowData(d);
        }

        public SeriesDetails ShowData { get; set; }

        public int ShowId { get; set; }

        public DirectoryInfo LibraryDirectory { get; set; }

        public string Name => LibraryDirectory.Name;
    }

}
