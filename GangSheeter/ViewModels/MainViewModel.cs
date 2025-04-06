using System.Collections.ObjectModel;
using GangSheeter.Models;
using GangSheeter.Services;
using GangSheeter.Views;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using System.IO;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace GangSheeter.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<ImageModel> Images { get; } = new ObservableCollection<ImageModel>();
        public PrintSheet CurrentSheet { get; } = new PrintSheet();

        public ICommand AddImagesCommand { get; }
        public ICommand GenerateSheetCommand { get; }
        public ICommand PrintCommand { get; }

        private void ExecutePrintCommand(object obj)
        {
            try
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.PrintSheet(CurrentSheet);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Printing failed: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private readonly ImageService _imageService;

        public MainViewModel()
        {
            _imageService = new ImageService();
            AddImagesCommand = new RelayCommand(AddImages);
            GenerateSheetCommand = new RelayCommand(GenerateSheet);
            PrintCommand = new RelayCommand(Print);
        }

        private void AddImages()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var newImages = _imageService.LoadImages(openFileDialog.FileNames);
                foreach (var image in newImages)
                {
                    Images.Add(image);
                }
            }
        }

        private void GenerateSheet()
        {
            CurrentSheet.Images.Clear();
            foreach (var image in Images)
            {
                CurrentSheet.AddImage(image);
            }
            // TODO: Implement ML-based layout optimization
        }

        private void Print()
        {
            try
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as Views.MainWindow;
                mainWindow?.PrintSheet(CurrentSheet);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Printing failed: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
