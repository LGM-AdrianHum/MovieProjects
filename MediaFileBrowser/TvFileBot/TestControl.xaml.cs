using System.Windows;
using TvDbSearchControl;

namespace TvFileBot
{
    /// <summary>
    ///     Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl
    {
        public TestControl()
        {
            InitializeComponent();
        }

        private void MySearch_OnTap(object sender, RoutedEventArgs e)
        {
            var args = e as TvDataRoutedEventArgs;
            if (args == null) return;
            MessageBox.Show(args.TvDbId.ToString());
        }
    }
}