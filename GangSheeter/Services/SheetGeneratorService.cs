using GangSheeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using GangSheeter.Services;

namespace GangSheeter.Services
{
    public class SheetGeneratorService : ISheetGeneratorService
    {
        private const double DefaultSheetWidthCm = 58.0;
        private const double DefaultSheetHeightCm = 100.0;
        private const double MaxSheetHeightCm = 1500.0;
        private const double MinSpacingCm = 1.0;
        private const double MaxSpacingCm = 5.0;
        private const double MarginCm = 0.5;

        public class ImagePlacement
        {
            public ImageInfo Image { get; set; } = null!;
            public double X { get; set; }
            public double Y { get; set; }
            public double Rotate { get; set; }
            public bool IsRotated { get; set; }
        }

        private readonly LayoutOptimizerML _mlOptimizer;

        public SheetGeneratorService()
        {
            _mlOptimizer = new LayoutOptimizerML();
        }

        public List<ImagePlacement> GenerateSheet(IEnumerable<ImageInfo> images)
        {
            if (images == null)
                return new List<ImagePlacement>();

            var placements = new List<ImagePlacement>();

            foreach (var image in images)
            {
                for (var i = 0; i < image.Copies; i++)
                {
                    var placement = new ImagePlacement
                    {
                        Image = image,
                    };
                    placements.Add(placement);
                }
            }

            _mlOptimizer.OptimizeLayout(placements, DefaultSheetWidthCm, MaxSheetHeightCm);

            return placements;
        }

        public void ReorganizeSheet(IEnumerable<ImagePlacement> placements)
        {
            _mlOptimizer.OptimizeLayout(placements);
        }
    }
}
