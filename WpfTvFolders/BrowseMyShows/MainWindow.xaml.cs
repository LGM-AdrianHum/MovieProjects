using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using TvHelpers;

namespace BrowseMyShows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    [ImplementPropertyChanged]
    public class ViewModel
    {
        public ViewModel()
        {
            var l = new List<DirectoryInfo>();
            for (var i = 1; i < 10; i++)
            {
                var d = new DirectoryInfo(@"q:\MyTv.Library\" + i.ToString("000"));
                if (d.Exists) l.AddRange(d.GetDirectories("*.*"));
            }
            AllDirs = l.Select(x => x.Name).OrderBy(x => x).Distinct(StringComparer.CurrentCultureIgnoreCase);
            AllCollected = l.GroupBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase).ToDictionary(x => x.Key, y => y.ToList());

            var r = new DirectoryInfo(Properties.Settings.Default.RawPath);
            if (r.Exists)
            {
                AllFiles = new ObservableCollection<RawFileData>(
                    FileNameHelpers.GetFiles(r).Select(x => new RawFileData(x)).OrderBy(x => x.TvFileInfo.Name));
            }
            else AllFiles = new ObservableCollection<RawFileData>();
        }

        public ObservableCollection<RawFileData> AllFiles { get; set; }

        public string TestText { get; set; }
        public IEnumerable<string> AllDirs { get; set; }
        public Dictionary<string, List<DirectoryInfo>> AllCollected { get; set; }
    }
}


