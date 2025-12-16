using AioStudy.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AioStudy.UI.Converter
{
    public class GradeToBackgroundConverter : IValueConverter
    {
        private static readonly Color FallbackColor = Color.FromRgb(107, 114, 128); // #6B7280

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(FallbackColor);
            }

            if (value is Module module)
            {
                if (module.IsKeyCompetence)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9F7AEA"));
                }

                if (module.Grade.HasValue)
                {
                    return GetBrushForGrade(module.Grade.Value);
                }

                return new SolidColorBrush(FallbackColor);
            }

            if (value is float f)
            {
                return GetBrushForGrade(f);
            }

            if (value is double d && float.TryParse(d.ToString(CultureInfo.InvariantCulture), out var parsed))
            {
                return GetBrushForGrade(parsed);
            }

            return new SolidColorBrush(FallbackColor);
        }

        private static SolidColorBrush GetBrushForGrade(float grade)
        {
            var color = grade switch
            {
                <= 0.85f => Color.FromRgb(34, 197, 94),   // #22C55E
                <= 1.15f => Color.FromRgb(74, 222, 128),  // #4ADE80
                <= 1.5f => Color.FromRgb(134, 239, 172),  // #86EFAC
                <= 1.85f => Color.FromRgb(94, 234, 212),  // #5EEAD4
                <= 2.15f => Color.FromRgb(45, 212, 191),  // #2DD4BF
                <= 2.5f => Color.FromRgb(103, 232, 249),  // #67E8F9
                <= 2.85f => Color.FromRgb(250, 204, 21),  // #FACC15
                <= 3.15f => Color.FromRgb(252, 211, 77),  // #FCD34D
                <= 3.5f => Color.FromRgb(251, 146, 60),   // #FB923C
                <= 3.85f => Color.FromRgb(249, 115, 22),  // #F97316
                <= 4.0f => Color.FromRgb(239, 68, 68),    // #EF4444
                _ => Color.FromRgb(220, 38, 38)           // #DC2626
            };
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}