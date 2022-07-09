using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageViewer.Source.MVVM.Model
{
    public class FilenameToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filePath = (string)value;

            if (filePath != null && File.Exists(filePath))
            {
                // Make sure the file's either a png or a jpg
                if (!ValidateImageFile(filePath))
                    return null;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(filePath);
                //image.DecodePixelWidth = 1920;
                image.EndInit();
                return image;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes sure that a file is either .png or a .jpg.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool ValidateImageFile(string filePath)
        {
            string[] parts = filePath.Split('.');
            string extension = parts[1];

            if (extension == "png" || extension == "jpg")
                return true;
            else
                return false;
        }
    }
}
