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
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = 0;
            if (value is int i) count = i;
            else if (value is long l) count = (int)l;
            else
            {
                try { count = System.Convert.ToInt32(value); } catch { count = 0; }
            }

            bool visible = count > 0;
            if (parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase))
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }
}
