using System.Collections.ObjectModel;
using System.Windows.Input;
using GangSheeter.Models;
using Microsoft.Win32;
using System.Windows;

namespace GangSheeter.ViewModels
{
    public class ImageViewModel
    {
        public ObservableCollection<ImageItem> Images { get; set; } = new ObservableCollection<ImageItem>();
        public ICommand UploadImageCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand RemoveCommand { get; }

        public ImageViewModel()
        {
            UploadImageCommand = new RelayCommand(UploadImage);
            ExitCommand = new RelayCommand(ExitApplication);
            RemoveCommand = new RelayCommand(RemoveImage);
        }

        private void UploadImage(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filename in openFileDialog.FileNames)
                {
                    // TODO: Add proper image processing
                    Images.Add(new ImageItem(filename, 1, 0, 0, 300));
                }
            }
        }

        private void ExitApplication(object parameter)
        {
            Application.Current.Shutdown();
        }

        private void RemoveImage(object parameter)
        {
            if (parameter is ImageItem image)
            {
                Images.Remove(image);
            }
        }

        public void AddImage(string filePath, int copies, double width, double height, int resolution)
        {
            var image = new ImageItem(filePath, copies, width, height, resolution);
            Images.Add(image);
        }
    }
}
