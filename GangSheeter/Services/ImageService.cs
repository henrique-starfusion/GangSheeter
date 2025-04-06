using GangSheeter.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows.Forms;

namespace GangSheeter.Services
{
    public class ImageService
    {
        public List<ImageModel> LoadImages(IEnumerable<string> filePaths)
        {
            return filePaths.Select(path => 
            {
                using var image = new Bitmap(path);
                return new ImageModel(path, new Bitmap(image));
            }).ToList();
        }

        public void SaveSheetAsTiff(PrintSheet sheet, string outputPath)
        {
            if (sheet.Images.Count == 0)
                throw new InvalidOperationException("No images to save");

            // Create encoder parameters for multi-page TIFF
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);

            // Get the TIFF encoder
            var tiffEncoder = GetEncoder(ImageFormat.Tiff);

            // Create first page
            using var firstImage = new Bitmap(sheet.Images[0].ImageData);
            if (tiffEncoder != null)
            {
                firstImage.Save(outputPath, tiffEncoder, encoderParams);
            }
            else
            {
                throw new InvalidOperationException("TIFF encoder not found");
            }

            // Add remaining pages
            encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
            for (int i = 1; i < sheet.Images.Count; i++)
            {
                using var img = new Bitmap(sheet.Images[i].ImageData);
                firstImage.SaveAdd(img, encoderParams);
            }

            // Close the file
            encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
            firstImage.SaveAdd(encoderParams);
        }

        public void PrintSheet(PrintSheet sheet)
        {
            // Printing will be handled by the View layer using WPF's printing capabilities
            throw new NotImplementedException("Printing should be implemented in the View layer using WPF printing");
        }

        private ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }

        public Bitmap GeneratePreview(PrintSheet sheet)
        {
            var preview = new Bitmap(
                (int)(sheet.WidthCm * sheet.Dpi / 2.54f),
                (int)(sheet.HeightCm * sheet.Dpi / 2.54f));

            using (var g = Graphics.FromImage(preview))
            {
                g.Clear(Color.White);
                float yPos = 0;
                
                foreach (var image in sheet.Images)
                {
                    var destRect = new RectangleF(
                        0, 
                        yPos, 
                        preview.Width, 
                        image.ImageData.Height * (preview.Width / (float)image.ImageData.Width));

                    g.DrawImage(image.ImageData, destRect);
                    yPos += destRect.Height + (sheet.Dpi / 2.54f); // Add 1cm spacing
                }
            }

            return preview;
        }
    }
}
