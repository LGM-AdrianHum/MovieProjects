using System.Linq;
using MasaSam.Controls;

namespace MasaSam.Samples.RatingControl
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void rtFive_RatingChanged(object sender, RatingChangedEventArgs e)
        {
            CalculateAverage();
        }

        private void rtTen_RatingChanged(object sender, RatingChangedEventArgs e)
        {
            CalculateAverage();
        }

        private void rtFifteen_RatingChanged(object sender, RatingChangedEventArgs e)
        {
            CalculateAverage();
        }

        private void CalculateAverage()
        {
            tbAverage.Text = "Average: " + new[] {rtFive.Value, rtTen.Value, rtFifteen.Value}.Average();
        }
    }
}