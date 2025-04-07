using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GangSheeter.Models;

public class ImageItem : ImageModel
{
    public BitmapImage Thumbnail { get; set; }
    public int Copies { get; set; } = 1;

    public ImageItem(string filePath, int copies, double width, double height, int resolution) : base(filePath, copies, width, height, resolution)
    {
        // Additional initialization if needed
    }

    public IEnumerable<ImageSheet> ToImageSheet()
    {
        for (int i = 0; i < this.Copies; i++)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(this.FilePath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            yield return new ImageSheet
            {
                Source = bitmap,
                Copy = i + 1,
                FilePath = FilePath,
                Width = Width,
                Height = Height,
                Resolution = Resolution,
            };
        }
    }

    public static ImageItem FromFile(string filePath)
    {
        using var image = Image.FromFile(filePath);

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(filePath);
        bitmap.DecodePixelWidth = 100; // thumbnail width
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze(); // Make it thread safe

        return new ImageItem(filePath, 1, image.Width / 100.0, image.Height / 100.0, 0);
    }
}
