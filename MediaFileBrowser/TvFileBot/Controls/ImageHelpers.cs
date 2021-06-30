using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TvFileBot.Controls
{
    public static class ImageHelpers
    {
        public static ImageSource CreateImageSource(string file, bool forcePreLoad)
        {
            if (forcePreLoad)
            {
                try
                {
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(file, UriKind.RelativeOrAbsolute);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    src.Freeze();
                    return src;

                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                var src = new BitmapImage(new Uri(file, UriKind.Absolute));
                src.Freeze();
                return src;
            }
        }

        public static BitmapSource ConvertBitmapTo96Dpi(BitmapImage bitmapImage)
        {
            const double dpi = 96;
            var width = bitmapImage.PixelWidth;
            var height = bitmapImage.PixelHeight;

            var stride = width * bitmapImage.Format.BitsPerPixel;
            var pixelData = new byte[stride * height];
            bitmapImage.CopyPixels(pixelData, stride, 0);

            return BitmapSource.Create(width, height, dpi, dpi, bitmapImage.Format, null, pixelData, stride);
        }

        public static void ResizeImage(Image img, double maxWidth, double maxHeight)
        {
            if (img?.Source == null) return;

            var srcWidth = img.Source.Width;
            var srcHeight = img.Source.Height;

            // Set your image tag to the sources DPI value for smart resizing if DPI != 96
            if (img.Tag != null && img.Tag.GetType() == typeof(double[]))
            {
                var dpi = (double[])img.Tag;
                srcWidth = srcWidth / (96 / dpi[0]);
                srcHeight = srcHeight / (96 / dpi[1]);
            }

            var resizedWidth = srcWidth;
            var resizedHeight = srcHeight;

            var aspect = srcWidth / srcHeight;

            if (resizedWidth > maxWidth)
            {
                resizedWidth = maxWidth;
                resizedHeight = resizedWidth / aspect;
            }
            if (resizedHeight > maxHeight)
            {
                aspect = resizedWidth / resizedHeight;
                resizedHeight = maxHeight;
                resizedWidth = resizedHeight * aspect;
            }

            img.Width = resizedWidth;
            img.Height = resizedHeight;
        }

    }
}