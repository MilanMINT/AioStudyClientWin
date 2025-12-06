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
    public class BoolToColorConverter : IValueConverter
    {
        public Brush TrueColor { get; set; } = Brushes.Green;
        public Brush FalseColor { get; set; } = Brushes.Red;
        public Brush NullColor { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? TrueColor : FalseColor;

            return NullColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
