using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class MinutesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (int)value == 0) return "0h 0m";
            if (!int.TryParse(value.ToString(), out var minutes)) return "N/A";
            var ts = TimeSpan.FromMinutes(minutes);
            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
