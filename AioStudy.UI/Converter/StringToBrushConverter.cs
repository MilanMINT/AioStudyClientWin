using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AioStudy.UI.Converter
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#608BC1"));

            try
            {
                var colorString = value.ToString();
                if (string.IsNullOrEmpty(colorString))
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#608BC1"));

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString!));
            }
            catch
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#608BC1"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color.ToString();
            }
            return "#608BC1";
        }
    }
}
