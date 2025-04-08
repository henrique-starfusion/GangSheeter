using System;
using System.Globalization;
using System.Windows.Data;

namespace GangSheeter.Converters
{
    public class BoolToRotationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isRotated && isRotated ? 90 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
