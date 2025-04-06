using System;
using System.Collections.Generic;
using System.Drawing;

namespace GangSheeter.Models
{
    public class PrintSheet
    {
        public const float DefaultWidthCm = 58f;
        public const float MaxHeightCm = 1500f;
        public const float MinSpacingCm = 1f;
        public const float MaxSpacingCm = 5f;

        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreatedDate { get; } = DateTime.Now;
        public float WidthCm { get; set; } = DefaultWidthCm;
        public float HeightCm { get; set; }
        public int Dpi { get; set; } = 300;
        public List<ImageModel> Images { get; } = new List<ImageModel>();
        public float EfficiencyScore { get; set; }

        public void AddImage(ImageModel image)
        {
            Images.Add(image);
            RecalculateSheetHeight();
        }

        public void RemoveImage(ImageModel image)
        {
            Images.Remove(image);
            RecalculateSheetHeight();
        }

        private void RecalculateSheetHeight()
        {
            // Basic height calculation - will be replaced with ML algorithm
            float totalHeight = 0;
            foreach (var image in Images)
            {
                totalHeight += image.HeightCm + MinSpacingCm;
            }
            HeightCm = Math.Min(totalHeight, MaxHeightCm);
        }

        public Bitmap GeneratePreview()
        {
            // TODO: Implement actual preview generation
            // This will create a bitmap representation of the sheet
            int widthPx = (int)(WidthCm * Dpi / 2.54);
            int heightPx = (int)(HeightCm * Dpi / 2.54);
            return new Bitmap(widthPx, heightPx);
        }
    }
}
