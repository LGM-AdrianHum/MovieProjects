using System;
using System.Windows;
using System.Windows.Controls;

namespace TvDbSearchControl
{
    /// <summary>
    ///     Interaction logic for TvDbSearch.xaml
    /// </summary>
    public partial class TvDbSearch
    {
        public static readonly RoutedEvent TapEvent = EventManager.RegisterRoutedEvent(
            "Tap", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TvDbSearch));


        public TvDbSearch()
        {
            InitializeComponent();
            Loaded += TvDbSearch_Loaded;
        }

        private void TvDbSearch_Loaded(object sender, RoutedEventArgs e)
        {
            //TextSearchValue.Text = QueryString;
        }

        //public static DependencyProperty QueryStringProperty = DependencyProperty.Register("QueryString", typeof(string), typeof(TvDbSearch), new UIPropertyMetadata(null));


        //public string QueryString
        //{
        //    get { return (string)GetValue(QueryStringProperty); }
        //    set
        //    {
        //        SetValue(QueryStringProperty, value);
                
        //        var dc = DataContext as SearchClass;
        //        if (dc?.SelectedResult == null) return;
        //        dc.SearchString = value;
        //    }
        //}
        
        // Provide CLR accessors for the event
        public event RoutedEventHandler Tap
        {
            add { AddHandler(TapEvent, value); }
            remove { RemoveHandler(TapEvent, value); }
        }

        // This method raises the Tap event
        private void RaiseTapEvent(int value)
        {
            var newEventArgs = new TvDataRoutedEventArgs(TapEvent, value);
            RaiseEvent(newEventArgs);
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as SearchClass;
            if (dc?.SelectedResult == null) return;
            RaiseTapEvent(dc.SelectedResult.Id);
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //var s = sender as TextBox;
            //if (s != null) QueryString = s.Text;
        }

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);
        //    if (e.Property == QueryStringProperty)
        //    {
        //        TextSearchValue.Text = QueryString;
        //        var dc = DataContext as SearchClass;
        //        if (dc == null) return;
        //        dc.SearchString = QueryString;
        //    }
        //}

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseTapEvent(0);
        }
    }

    public class TvDataRoutedEventArgs : RoutedEventArgs
    {
        public TvDataRoutedEventArgs(RoutedEvent routedEvent, int tvDbId) : base(routedEvent)
        {
            TvDbId = tvDbId;
        }

        public int TvDbId { get; private set; }
    }
}