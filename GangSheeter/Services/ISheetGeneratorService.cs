using GangSheeter.Models;
using System.Collections.Generic;

namespace GangSheeter.Services
{
    public interface ISheetGeneratorService
    {
        // Updated references from ImagePlacement to ImageSheet
        List<ImageSheet> GenerateSheet(IEnumerable<ImageInfo> images);

        void ReorganizeSheet(IEnumerable<ImageSheet> placements);
    }
}
