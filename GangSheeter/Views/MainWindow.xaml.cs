using System.Windows;
using Microsoft.Win32;
using GangSheeter.ViewModels;
using System.IO;
using System.Drawing;

namespace GangSheeter.Views
{
    public partial class MainWindow : Window
    {
        private ImageViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new ImageViewModel();
            DataContext = _viewModel;
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    using (var image = Image.FromFile(filePath))
                    {
                        double width = image.Width / 100.0; // Convert pixels to centimeters
                        double height = image.Height / 100.0; // Convert pixels to centimeters
                        _viewModel.AddImage(filePath, 1, width, height, 300); // Example values
                    }
                }
            }
        }
    }
}
