using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MovieNamer{
    public sealed class UriToCachedImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(Application.Current is App)) return null;

            var url = value as string;
            if (string.IsNullOrEmpty(url))
                return null;

            var webUri = new Uri(url, UriKind.Absolute);
            var filename = Path.GetFileName(webUri.AbsolutePath);

            var localFilePath = Path.Combine(@"\\admin-pc\g\MovieDataLibrary\Backdrops", filename);

            if (File.Exists(localFilePath))
            {
                return BitmapFrame.Create(new Uri(localFilePath, UriKind.Absolute));
            }

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

        public void SaveImage(BitmapImage image, string localFilePath)
        {
            image.DownloadCompleted += (sender, args) =>
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage) sender));
                using (var filestream = new FileStream(localFilePath, FileMode.Create))
                {
                    encoder.Save(filestream);
                }
            };
        }
    }}

//<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
//             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
//             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
//             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
//             xmlns:u="clr-namespace:MyNamespace"         
//             d:DesignHeight="500" 
//             d:DesignWidth="420">
//    <UserControl.Resources>
//        <ResourceDictionary>
//            <u:UriToCachedImageConverter x:Key="UrlToCachedImageConverter" />
//        </ResourceDictionary>
//    </UserControl.Resources>           
//</UserControl>

//<Image Source="{Binding URL, Mode=OneWay, Converter={StaticResource UrlToCachedImageConverter}, IsAsync=true}"/>
