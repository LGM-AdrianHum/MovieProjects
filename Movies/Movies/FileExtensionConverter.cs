using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Movies {
    public class FileExtensionConverter : IValueConverter {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var r = FileExtensions.Instance;
            var requestImage = new Image()
            {
                Height = 16,
                Width = 16,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            };
            var rr = "plain.ico";
            if (value != null)
            {
                string name = ((string) value).ToLower();
                
                var j = r.ResourceList.FirstOrDefault(x => x.StartsWith(name));
                if (!string.IsNullOrEmpty(j)) rr = j;
                
            }
            //                                             pack://application:,,,/images/264.png
            requestImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/" + rr,UriKind.RelativeOrAbsolute));
            return requestImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}