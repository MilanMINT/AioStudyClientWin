using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class ProgressToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return 0.0;

            if (!double.TryParse(values[0]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                value = 0.0;
            if (!double.TryParse(values[1]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double maximum) || maximum <= 0.0)
                maximum = 100.0;
            if (!double.TryParse(values[2]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double totalWidth))
                totalWidth = 0.0;

            double ratio = Math.Max(0.0, Math.Min(1.0, value / maximum));
            return ratio * totalWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
