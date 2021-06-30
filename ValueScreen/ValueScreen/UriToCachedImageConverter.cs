using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ValueScreen
{
    public class UriToCachedImageConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <exception cref="FileNotFoundException">Condition.</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var localcachepath = @"k:\ImageCache";
            if (string.IsNullOrEmpty(localcachepath)) localcachepath = @"C:\MyImagesFolder\";
            if (!Directory.Exists(localcachepath)) Directory.CreateDirectory(localcachepath);

            var url = value as string;
            if (url == null)
                return null;

            var webUri = new Uri(url, UriKind.Absolute);
            var filename = Path.GetFileName(webUri.AbsolutePath);

            var localFilePath = Path.Combine(localcachepath, filename);

            if (File.Exists(localFilePath))
                return BitmapFrame.Create(new Uri(localFilePath, UriKind.Absolute));

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = webUri;
            image.EndInit();

            SaveImage(image, localFilePath);

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void SaveImage(BitmapImage image, string localFilePath)
        {
            image.DownloadCompleted += (sender, args) => {
                                                             var encoder = new PngBitmapEncoder();
                                                             encoder.Frames.Add(BitmapFrame.Create((BitmapImage)sender));
                                                             using (var filestream = new FileStream(localFilePath, FileMode.Create)) encoder.Save(filestream);
            };
        }
    }
}