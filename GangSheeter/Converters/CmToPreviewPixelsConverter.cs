using System;
using System.Globalization;
using System.Windows.Data;

namespace GangSheeter.Converters
{
    public class CmToPreviewPixelsConverter : IValueConverter
    {
        private const double PixelsPerCm = 10.0; // 10px per cm for preview

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double cm)
            {
                return cm * PixelsPerCm;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double pixels)
            {
                return pixels / PixelsPerCm;
            }
            return 0;
        }
    }
}
