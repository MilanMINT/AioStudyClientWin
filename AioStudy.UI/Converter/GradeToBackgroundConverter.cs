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
                return new SolidColorBrush(Color.FromRgb(107, 114, 128)); // #6B7280 Grau (Fallback)
            }

            if (value is float grade || (value is double d && float.TryParse(d.ToString(), out grade)))
            {
                var color = grade switch
                {
                    <= 0.85f => Color.FromRgb(34, 197, 94),   // #22C55E Dunkelgrün (0.7)
                    <= 1.15f => Color.FromRgb(74, 222, 128),  // #4ADE80 Grün (1.0)
                    <= 1.5f => Color.FromRgb(134, 239, 172),  // #86EFAC Hellgrün (1.3)
                    <= 1.85f => Color.FromRgb(94, 234, 212),  // #5EEAD4 Türkis-Grün (1.7)
                    <= 2.15f => Color.FromRgb(45, 212, 191),  // #2DD4BF Türkis (2.0)
                    <= 2.5f => Color.FromRgb(103, 232, 249),  // #67E8F9 Hell-Türkis (2.3)
                    <= 2.85f => Color.FromRgb(250, 204, 21),  // #FACC15 Gelb (2.7)
                    <= 3.15f => Color.FromRgb(252, 211, 77),  // #FCD34D Hellgelb (3.0)
                    <= 3.5f => Color.FromRgb(251, 146, 60),   // #FB923C Orange (3.3)
                    <= 3.85f => Color.FromRgb(249, 115, 22),  // #F97316 Dunkelorange (3.7)
                    <= 4.0f => Color.FromRgb(239, 68, 68),    // #EF4444 Rot (4.0)
                    _ => Color.FromRgb(220, 38, 38)           // #DC2626 Dunkelrot (5.0)
                };
                return new SolidColorBrush(color);
            }

            return new SolidColorBrush(Color.FromRgb(107, 114, 128)); // #6B7280 Grau (Fallback)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
