using System.Collections.ObjectModel;
using GangSheeter.Models;

namespace GangSheeter.ViewModels
{
    public class ImageViewModel
    {
        public ObservableCollection<ImageItem> Images { get; set; } = new ObservableCollection<ImageItem>();

        public ImageViewModel()
        {
        }

        public void AddImage(string filePath, int copies, double width, double height, int resolution)
        {
            var image = new ImageItem(filePath, copies, width, height, resolution);
            Images.Add(image);
        }

        public void RemoveImage(ImageItem image) => Images.Remove(image);
    }
}
