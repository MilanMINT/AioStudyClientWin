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
    public class GradeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Color.FromRgb(159, 122, 234)); // #9F7AEA
            }

            if (value is float grade || (value is double d && float.TryParse(d.ToString(), out grade)))
            {
                var color = grade switch
                {
                    <= 1.0f => Color.FromRgb(34, 197, 94),   // #22C55E
                    <= 1.5f => Color.FromRgb(74, 222, 128),  // #4ADE80
                    <= 2.0f => Color.FromRgb(45, 212, 191),  // #2DD4BF
                    <= 2.5f => Color.FromRgb(103, 232, 249), // #67E8F9
                    <= 3.0f => Color.FromRgb(250, 204, 21),  // #FACC15
                    <= 3.5f => Color.FromRgb(251, 146, 60),  // #FB923C
                    <= 4.0f => Color.FromRgb(239, 68, 68),   // #EF4444
                    _ => Color.FromRgb(220, 38, 38)          // #DC2626
                };
                return new SolidColorBrush(color);
            }

            return new SolidColorBrush(Color.FromRgb(159, 122, 234));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
