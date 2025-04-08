using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using Fluent;

namespace GangSheeter
{
    public partial class MainWindow : RibbonWindow
    {
        public ObservableCollection<ImageItem> Images { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Images = new ObservableCollection<ImageItem>();
            ImagesGrid.ItemsSource = Images;
            UpdateStatus("Ready");
        }

        private void UpdateStatus(string message)
        {
            StatusText.Text = message;
        }

        private void OnSelectImagesClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image files (*.png;*.tiff)|*.png;*.tiff"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                UpdateStatus("Loading images...");
                int successCount = 0;

                foreach (string filename in openFileDialog.FileNames)
                {
                    try
                    {
                        var bitmap = new BitmapImage(new Uri(filename));
                        var thumbnailPath = GenerateThumbnail(bitmap);

                        var imageItem = new ImageItem
                        {
                            OriginalPath = filename,
                            ThumbnailPath = thumbnailPath,
                            Copies = 1,
                            Width = bitmap.PixelWidth,
                            Height = bitmap.PixelHeight,
                            Resolution = $"{bitmap.DpiX}x{bitmap.DpiY} DPI"
                        };

                        Images.Add(imageItem);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image {filename}: {ex.Message}");
                    }
                }

                UpdateStatus($"Loaded {successCount} image(s) successfully");
            }
        }

        private string GenerateThumbnail(BitmapImage originalImage)
        {
            var scale = Math.Min(50.0 / originalImage.PixelWidth, 50.0 / originalImage.PixelHeight);
            var thumbnail = new TransformedBitmap(originalImage, 
                new ScaleTransform(scale, scale));

            var thumbnailPath = Path.Combine(Path.GetTempPath(), 
                $"thumbnail_{Guid.NewGuid()}.png");

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(thumbnail));

            using (var fileStream = new FileStream(thumbnailPath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }

            return thumbnailPath;
        }

        private void OnDeleteImageClick(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            var imageItem = (ImageItem)button.DataContext;

            try
            {
                if (File.Exists(imageItem.ThumbnailPath))
                {
                    File.Delete(imageItem.ThumbnailPath);
                }
                Images.Remove(imageItem);
                UpdateStatus($"Image deleted. {Images.Count} image(s) remaining");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting image: {ex.Message}");
                UpdateStatus("Error deleting image");
            }
        }
    }

    public class ImageItem
    {
        public required string OriginalPath { get; set; }
        public required string ThumbnailPath { get; set; }
        public int Copies { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public required string Resolution { get; set; }
    }
}
