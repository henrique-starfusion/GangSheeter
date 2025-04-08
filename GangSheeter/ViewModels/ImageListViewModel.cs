using GangSheeter.Models;
using GangSheeter.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;

namespace GangSheeter.ViewModels
{
    public class ImageListViewModel : INotifyPropertyChanged
    {
        private readonly SheetGeneratorService _sheetGenerator;
        private ObservableCollection<ImageInfo> _images = new();
        private ImageInfo? _selectedImage;
        private ICommand? _addImagesCommand;
        private ICommand? _removeImageCommand;
        private bool _isLoadingImages;

        public bool IsLoadingImages
        {
            get => _isLoadingImages;
            set => SetField(ref _isLoadingImages, value);
        }

        public ObservableCollection<ImageInfo> Images
        {
            get => _images;
            set => SetField(ref _images, value);
        }

        public ImageInfo? SelectedImage
        {
            get => _selectedImage;
            set => SetField(ref _selectedImage, value);
        }

        public ICommand AddImagesCommand => _addImagesCommand ??= new RelayCommand(AddImages);
        public ICommand RemoveImageCommand => _removeImageCommand ??= new RelayCommand<ImageInfo>(RemoveImage);

        public ImageListViewModel(SheetGeneratorService sheetGenerator)
        {
            _sheetGenerator = sheetGenerator;
        }

        private async void AddImages()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsLoadingImages = true;
                    await Task.Run(() =>
                    {
                        foreach (var filePath in dialog.FileNames)
                        {
                            try
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var bitmap = new BitmapImage();
                                    bitmap.BeginInit();
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmap.UriSource = new Uri(filePath);
                                    bitmap.EndInit();
                                    bitmap.Freeze(); // Make it usable across threads

                                    var imageInfo = new ImageInfo
                                    {
                                        FilePath = filePath,
                                        FileName = System.IO.Path.GetFileName(filePath),
                                        WidthCm = bitmap.PixelWidth * 2.54 / 300, // Convert pixels to cm at 300 DPI
                                        HeightCm = bitmap.PixelHeight * 2.54 / 300,
                                        DPI = 300
                                    };
                                    Images.Add(imageInfo);
                                });
                            }
                            catch (Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show($"Error loading image {filePath}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                });
                            }
                        }
                    });
                }
                finally
                {
                    IsLoadingImages = false;
                }
            }
        }

        private void RemoveImage(ImageInfo? imageToRemove)
        {
            if (imageToRemove != null)
            {
                Images.Remove(imageToRemove);
            }
        }

        public void UpdateImageCopies(ImageInfo image, int copies)
        {
            var existing = Images.FirstOrDefault(i => i.Id == image.Id);
            if (existing != null)
            {
                existing.Copies = copies;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
