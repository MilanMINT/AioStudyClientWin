using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class DateTimeNullableDateOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly d)
                return d.ToDateTime(TimeOnly.MinValue);
            if (value is DateTime dt)
                return dt;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return DateOnly.FromDateTime(dt);
            if (value is null)
                return Binding.DoNothing;
            return Binding.DoNothing;
        }
    }
}
