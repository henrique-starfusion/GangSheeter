using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GangSheeter.Services;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace GangSheeter.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SheetGeneratorService _sheetGenerator;
        private readonly LayoutOptimizerML _mlOptimizer;
        private ImageListViewModel _imageList = null!;
        private ObservableCollection<ImageSheet> _placements = new();
        private ICommand? _generateSheetCommand;
        private ICommand? _reorganizeSheetCommand;
        private ICommand? _saveCommand;
        private bool _isGeneratingSheet;
        private bool _isReorganizeSheet;

        public bool IsGeneratingSheet
        {
            get => _isGeneratingSheet;
            set => SetField(ref _isGeneratingSheet, value);
        }

        public bool IsReorganizeSheet
        {
            get => _isReorganizeSheet;
            set => SetField(ref _isReorganizeSheet, value);
        }

        public ImageListViewModel ImageList
        {
            get => _imageList;
            set => SetField(ref _imageList, value);
        }

        public ObservableCollection<ImageSheet> Placements
        {
            get => _placements;
            set => SetField(ref _placements, value);
        }

        public ICommand GenerateSheetCommand => _generateSheetCommand ??= new RelayCommand(GenerateSheet, CanGenerateSheet);
        public ICommand ReorfanizeSheetCommand => _reorganizeSheetCommand ??= new RelayCommand(ReorganizeSheet, CanReorganizeSheet);
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save, CanSave);

        public MainViewModel(ImageListViewModel imageList, SheetGeneratorService sheetGenerator, LayoutOptimizerML mlOptimizer)
        {
            _imageList = imageList;
            _sheetGenerator = sheetGenerator;
            _mlOptimizer = mlOptimizer;
        }

        private bool CanGenerateSheet()
        {
            return ImageList.Images.Any();
        }

        private bool CanReorganizeSheet()
        {
            return Placements.Any();
        }

        private async void GenerateSheet()
        {
            try
            {
                IsGeneratingSheet = true;
                await Task.Run(() =>
                {
                    var placements = _sheetGenerator.GenerateSheet(ImageList.Images);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Placements.Clear();
                        if (placements != null)
                        {
                            foreach (var placement in placements.Where(p => p?.Image != null))
                            {
                                Placements.Add(placement);
                            }
                        }
                        _sheetGenerator.ReorganizeSheet(Placements);
                    });
                });
            }
            finally
            {
                IsGeneratingSheet = false;
            }
        }

        private async void ReorganizeSheet()
        {
            try
            {
                IsReorganizeSheet = true;

                await Task.Run(() =>
                   {
                       Application.Current.Dispatcher.Invoke(() =>
                       {
                           _sheetGenerator.ReorganizeSheet(Placements);
                       });
                   });
            }
            finally
            {
                IsReorganizeSheet = false;
            }
        }

        private bool CanSave()
        {
            return Placements.Any();
        }

        private void Save()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "TIFF Image|*.tif;*.tiff",
                DefaultExt = ".tiff"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Create a new RenderTargetBitmap at 300 DPI
                    var dpi = 300;
                    var width = (int)(58 * dpi / 2.54); // 58cm to pixels at 300 DPI
                    var height = (int)(150 * dpi / 2.54); // 150cm to pixels at 300 DPI

                    var renderTarget = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);

                    // Create a DrawingVisual to render the sheet
                    var visual = new DrawingVisual();
                    using (var context = visual.RenderOpen())
                    {
                        foreach (var placement in Placements)
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.UriSource = new Uri(placement.Image.FilePath);
                            image.EndInit();
                            image.Freeze(); // Make it usable across threads

                            var x = placement.X * dpi / 2.54;
                            var y = placement.Y * dpi / 2.54;
                            var transform = new TransformGroup();

                            if (placement.IsRotated)
                            {
                                transform.Children.Add(new RotateTransform(90, image.PixelWidth / 2, image.PixelHeight / 2));
                            }

                            transform.Children.Add(new TranslateTransform(x, y));
                            context.PushTransform(transform);
                            context.DrawImage(image, new Rect(0, 0, image.PixelWidth, image.PixelHeight));
                            context.Pop();
                        }
                    }

                    renderTarget.Render(visual);

                    // Save as TIFF
                    var encoder = new TiffBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                    using (var stream = File.Create(dialog.FileName))
                    {
                        encoder.Save(stream);
                    }

                    MessageBox.Show("Sheet exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting sheet: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
