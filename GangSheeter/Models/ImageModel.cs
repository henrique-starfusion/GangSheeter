using System;

namespace GangSheeter.Models
{
    public class ImageModel
    {
        public string FilePath { get; set; }
        public int Copies { get; set; }
        public double Width { get; set; } // in centimeters
        public double Height { get; set; } // in centimeters
        public int Resolution { get; set; } // in DPI

        public ImageModel(string filePath, int copies, double width, double height, int resolution)
        {
            FilePath = filePath;
            Copies = copies;
            Width = width;
            Height = height;
            Resolution = resolution;
        }

        // Default constructor for cases where parameters are not provided
        public ImageModel() { }
    }
}
