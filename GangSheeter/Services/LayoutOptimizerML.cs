using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using GangSheeter.Models;
using System.Collections.Generic;
using System.Linq;

namespace GangSheeter.Services
{
    public class LayoutOptimizerML
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public LayoutOptimizerML()
        {
            _mlContext = new MLContext();
            // Initialize with empty model
            var emptyData = _mlContext.Data.LoadFromEnumerable(new List<LayoutData>());
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(LayoutData.ImageWidth),
                    nameof(LayoutData.ImageHeight),
                    nameof(LayoutData.IsRotated));
            _model = pipeline.Fit(emptyData);
        }

        public void TrainModel(IEnumerable<SheetGeneratorService.ImagePlacement> historicalData)
        {
            // Prepare data
            var data = _mlContext.Data.LoadFromEnumerable(historicalData
                .Select(p => new LayoutData
                {
                    ImageWidth = (float)(p.IsRotated ? p.Image.HeightCm : p.Image.WidthCm),
                    ImageHeight = (float)(p.IsRotated ? p.Image.WidthCm : p.Image.HeightCm),
                    CurrentY = (float)p.Y,
                    IsRotated = p.IsRotated,
                    EfficiencyScore = CalculateEfficiencyScore(p)
                }));

            // Define pipeline
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(LayoutData.ImageWidth),
                    nameof(LayoutData.ImageHeight),
                    nameof(LayoutData.IsRotated))
                .Append(_mlContext.Regression.Trainers.Sdca());

            // Train model
            _model = pipeline.Fit(data);
        }

        public void OptimizeLayout(
            IEnumerable<SheetGeneratorService.ImagePlacement> placements,
            double sheetWidth = 58.0,
            double maxSheetHeight = 1500.0)
        {
            try
            {
                if (placements == null || _model == null)
                    return;

                // Create prediction engine with error handling
                var engine = _mlContext.Model.CreatePredictionEngine<LayoutData, LayoutPrediction>(_model);
                if (engine == null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create prediction engine");
                    return;
                }

                double currentY = 0;
                double currentX = 0;

                foreach (var placement in placements)
                {
                    bool shouldRotate = false;
                    try
                    {
                        var prediction = engine.Predict(new LayoutData
                        {
                            ImageWidth = (float)placement.Image.WidthCm,
                            ImageHeight = (float)placement.Image.HeightCm
                        });
                        shouldRotate = prediction != null && prediction.PredictedRotation;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Prediction failed for image: {ex.Message}");
                        shouldRotate = placement.Image.WidthCm > placement.Image.HeightCm; // Simple aspect ratio fallback
                    }
                    double width = shouldRotate ? placement.Image.HeightCm : placement.Image.WidthCm;
                    double height = shouldRotate ? placement.Image.WidthCm : placement.Image.HeightCm;

                    if (width > sheetWidth && height > sheetWidth)
                        throw new InvalidOperationException("Exceeded maximum sheet width.");

                    if (width > sheetWidth)
                        shouldRotate = true;

                    placement.X = currentX;
                    placement.Y = currentY;
                    placement.IsRotated = shouldRotate;

                    if (shouldRotate)
                        placement.Rotate = 90;

                    if (currentY + height > maxSheetHeight)
                    {
                        throw new InvalidOperationException("Exceeded maximum sheet height.");
                    }

                    currentY += height;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Optimization analysis failed: {ex.Message}");
            }
        }

        private class LayoutPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool PredictedRotation { get; set; }

            public float Probability { get; set; }

            public float Score { get; set; }
        }

        private float CalculateEfficiencyScore(SheetGeneratorService.ImagePlacement placement)
        {
            // Calculate space utilization efficiency (0-1)
            double usedWidth = placement.X + (placement.IsRotated ? placement.Image.HeightCm : placement.Image.WidthCm);
            double usedHeight = placement.Y + (placement.IsRotated ? placement.Image.WidthCm : placement.Image.HeightCm);
            return (float)((usedWidth / 58.0) * (usedHeight / 1500.0));
        }

        private class LayoutData
        {
            public float ImageWidth { get; set; }
            public float ImageHeight { get; set; }
            public float SheetWidth { get; set; } = 58f;
            public float CurrentY { get; set; }
            public bool IsRotated { get; set; }
            public float EfficiencyScore { get; set; }
        }
    }
}
