using Material.Icons;
using Material.Icons.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class BoolToIconConverterSingle : IValueConverter
    {

        public MaterialIconKind TrueIcon { get; set; } = MaterialIconKind.Check;
        public MaterialIconKind FalseIcon { get; set; } = MaterialIconKind.Close;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueIcon : FalseIcon;
            } 
            return FalseIcon; // Fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
