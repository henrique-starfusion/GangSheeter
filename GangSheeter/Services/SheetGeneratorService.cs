using GangSheeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using GangSheeter.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        
        private readonly LayoutOptimizerML _mlOptimizer;

        public SheetGeneratorService()
        {
            _mlOptimizer = new LayoutOptimizerML();
        }

        public List<ImageSheet> GenerateSheet(IEnumerable<ImageInfo> images)
        {
            if (images == null)
                return new List<ImageSheet>();

            var placements = new List<ImageSheet>();

            foreach (var image in images)
            {
                for (var i = 0; i < image.Copies; i++)
                {
                    var placement = new ImageSheet
                    {
                        Image = image,
                    };
                    placements.Add(placement);
                }
            }

            _mlOptimizer.OptimizeLayout(placements, DefaultSheetWidthCm, MaxSheetHeightCm);

            return placements;
        }

        public void ReorganizeSheet(IEnumerable<ImageSheet> placements)
        {
            _mlOptimizer.OptimizeLayout(placements);
        }
    }
}
