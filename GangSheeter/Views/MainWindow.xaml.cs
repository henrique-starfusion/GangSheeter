using GangSheeter.ViewModels;
using GangSheeter.Models;
using System;
using System.Windows;
using Window = System.Windows.Window;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;

namespace GangSheeter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // InitializeComponent is called automatically when the XAML is parsed
            // Remove the explicit DataContext setting since it's set in XAML
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Window loaded logic
        }

        public void PrintSheet(PrintSheet sheet)
        {
            if (sheet == null) return;

            var printDialog = new System.Windows.Controls.PrintDialog();
            
            if (printDialog.ShowDialog() == true)
            {
                var document = new FixedDocument();
                var pageContent = new PageContent();
                var fixedPage = new FixedPage();

                var canvas = new Canvas
                {
                    Width = printDialog.PrintableAreaWidth,
                    Height = printDialog.PrintableAreaHeight,
                    Background = System.Windows.Media.Brushes.White
                };

                double yPos = 0;
                foreach (var image in sheet.Images)
                {
                    var bitmap = ConvertDrawingImageToBitmapSource(image.ImageData);
                    var wpfImage = new System.Windows.Controls.Image
                    {
                        Source = bitmap,
                        Width = printDialog.PrintableAreaWidth,
                        Height = (bitmap.PixelHeight * printDialog.PrintableAreaWidth) / bitmap.PixelWidth
                    };

                    Canvas.SetTop(wpfImage, yPos);
                    canvas.Children.Add(wpfImage);
                    yPos += wpfImage.Height + (sheet.Dpi / 2.54); // 1cm spacing
                }

                fixedPage.Children.Add(canvas);
                pageContent.Child = fixedPage;
                document.Pages.Add(pageContent);

                printDialog.PrintDocument(document.DocumentPaginator, "Gang Sheet");
            }
        }

        private BitmapSource ConvertDrawingImageToBitmapSource(Drawing.Image img)
        {
            var bitmap = new BitmapImage();
            using (var ms = new System.IO.MemoryStream())
            {
                img.Save(ms, Drawing.Imaging.ImageFormat.Png);
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            return bitmap;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = DataContext as MainViewModel;
                if (viewModel?.CurrentSheet != null)
                {
                    PrintSheet(viewModel.CurrentSheet);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Printing failed: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
