using System;
using System.Globalization;
using System.Web;
using System.Windows;
using System.Windows.Data;

namespace MovieNamer {
    public sealed class HtmlEncodeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv = value as string;
            if (string.IsNullOrEmpty(conv))
                return null;
            return HttpUtility.HtmlDecode(conv);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv = value as string;
            if (string.IsNullOrEmpty(conv))
                return null;
            return HttpUtility.HtmlEncode(conv);
        }
    }

    public sealed class StringNullToVisibility : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            return string.IsNullOrEmpty(val) ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}