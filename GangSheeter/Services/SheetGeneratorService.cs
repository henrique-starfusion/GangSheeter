using GangSheeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GangSheeter.Services
{
    public class SheetGeneratorService
    {
        private const double DefaultSheetWidthCm = 58.0;
        private const double DefaultSheetHeightCm = 150.0;
        private const double MinSpacingCm = 1.0;
        private const double MaxSpacingCm = 5.0;
        private const double MarginCm = 0.5;

        public class ImagePlacement
        {
            public ImageInfo Image { get; set; } = null!;
            public double X { get; set; }
            public double Y { get; set; }
            public bool IsRotated { get; set; }
        }

        public List<ImagePlacement> GenerateSheet(IEnumerable<ImageInfo> images)
        {
            var placements = new List<ImagePlacement>();
            var currentY = MarginCm;
            var currentX = MarginCm;
            var rowHeight = 0.0;

            foreach (var image in images)
            {
                for (int i = 0; i < image.Copies; i++)
                {
                    var (fits, isRotated) = TryFitImage(image, currentX, currentY, DefaultSheetWidthCm);
                    
                    if (!fits && currentX > MarginCm)
                    {
                        // Start new row
                        currentX = MarginCm;
                        currentY += rowHeight + MinSpacingCm;
                        rowHeight = 0;
                        (fits, isRotated) = TryFitImage(image, currentX, currentY, DefaultSheetWidthCm);
                    }

                    if (fits)
                    {
                        var placement = new ImagePlacement
                        {
                            Image = image,
                            X = currentX,
                            Y = currentY,
                            IsRotated = isRotated
                        };
                        placements.Add(placement);

                        // Update position
                        var imageWidth = isRotated ? image.HeightCm : image.WidthCm;
                        var imageHeight = isRotated ? image.WidthCm : image.HeightCm;
                        currentX += imageWidth + MinSpacingCm;
                        rowHeight = Math.Max(rowHeight, imageHeight);
                    }
                }
            }

            return placements;
        }

        private (bool fits, bool isRotated) TryFitImage(ImageInfo image, double x, double y, double sheetWidth)
        {
            // Try normal orientation
            if (x + image.WidthCm <= sheetWidth - MarginCm)
                return (true, false);

            // Try rotated
            if (x + image.HeightCm <= sheetWidth - MarginCm)
                return (true, true);

            return (false, false);
        }
    }
}
