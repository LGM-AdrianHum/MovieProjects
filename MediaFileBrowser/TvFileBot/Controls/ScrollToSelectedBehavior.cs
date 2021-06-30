using System.Windows;
using System.Windows.Controls;

namespace TvFileBot.Controls
{
    public static class ScrollToSelectedBehavior
    {
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.RegisterAttached(
            "SelectedValue",
            typeof(object),
            typeof(ScrollToSelectedBehavior),
            new PropertyMetadata(null, OnSelectedValueChange));

        public static void SetSelectedValue(DependencyObject source, object value)
        {
            source.SetValue(SelectedValueProperty, value);
        }

        public static object GetSelectedValue(DependencyObject source)
        {
            return (object)source.GetValue(SelectedValueProperty);
        }

        private static void OnSelectedValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var lb = d as ListBox;
            var si = lb?.Items[0];
            lb?.ScrollIntoView(si);
        }
    }
}
