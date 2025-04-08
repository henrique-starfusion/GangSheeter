using GangSheeter.Models;
using System.Collections.Generic;

namespace GangSheeter.Services
{
    public interface ISheetGeneratorService
    {
        List<SheetGeneratorService.ImagePlacement> GenerateSheet(IEnumerable<ImageInfo> images);

        void ReorganizeSheet(IEnumerable<SheetGeneratorService.ImagePlacement> placements);
    }
}
