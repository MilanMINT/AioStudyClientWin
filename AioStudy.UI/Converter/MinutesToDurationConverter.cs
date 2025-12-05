using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class MinutesToDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int minutes)
            {
                if (minutes < 60)
                    return $"{minutes}m";

                int hours = minutes / 60;
                int mins = minutes % 60;

                return mins > 0 ? $"{hours}h {mins}m" : $"{hours}h";
            }
            return "0m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
