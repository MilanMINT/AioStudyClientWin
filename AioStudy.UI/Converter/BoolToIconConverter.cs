using Material.Icons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class BoolToIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isRunning = (bool)values[0];
            bool isPaused = (bool)values[1];

            if (!isRunning && !isPaused) return MaterialIconKind.Play; // Stopped
            if (isRunning && !isPaused) return MaterialIconKind.Pause; // Running
            if (!isRunning && isPaused) return MaterialIconKind.Play; // Paused
            return MaterialIconKind.Play; // Fallback
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
