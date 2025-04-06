using System;
using System.Drawing;

namespace GangSheeter.Models
{
    public class ImageModel
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string FilePath { get; set; }
        public Bitmap ImageData { get; set; }
        public int Copies { get; set; } = 1;
        public float WidthCm { get; set; }
        public float HeightCm { get; set; }
        public int Dpi { get; set; } = 300;
        public float Rotation { get; set; }
        public PointF Position { get; set; }

        public ImageModel(string filePath, Bitmap imageData)
        {
            FilePath = filePath;
            ImageData = imageData;
            // Calculate dimensions in cm based on DPI
            WidthCm = imageData.Width / (imageData.HorizontalResolution / 2.54f);
            HeightCm = imageData.Height / (imageData.VerticalResolution / 2.54f);
        }
    }
}
