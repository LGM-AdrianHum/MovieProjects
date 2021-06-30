using System;
using System.IO;
using System.Windows;

namespace ValueScreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DirectoryHelper _dm;

        public MainWindow()
        {
            InitializeComponent();
            _dm = new DirectoryHelper();
            DataContext = _dm;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            DateBlock.Text = DateTime.Now.ToLongDateString();
            _dm.CurrentDirectory = new DirInfo(_dm, new DirectoryInfo(@"k:\btsync"));
            _dm.RefreshCurrentItems();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeBlock.Text = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}