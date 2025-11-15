using Material.Icons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class BoolToIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null ||
                values.Length != 2 ||
                values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue ||
                !(values[0] is bool) ||
                !(values[1] is bool))
            {
                return MaterialIconKind.Play; // Fallback
            }

            bool isRunning = (bool)values[0];
            bool isPaused = (bool)values[1];

            if (isRunning && !isPaused)
                return MaterialIconKind.Pause;

            return MaterialIconKind.Play;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
