using AioStudy.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class GradeToDisplayTextConverter : IValueConverter
    {
        private const string NoGradeFallback = "BE";
        private const string KeyText = "Key";
        private const string OpenText = "Open";
        private const string NotEvaluatedText = "NB";
        private const float NotEvaluatedGrade = 5.0f;
        private const float Epsilon = 0.001f;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Module module)
            {
                if (module.Grade.HasValue)
                {
                    var g = module.Grade.Value;
                    if (Math.Abs(g - NotEvaluatedGrade) < Epsilon)
                        return NotEvaluatedText;

                    return g.ToString("F1", CultureInfo.InvariantCulture).Replace(".", ",");
                }

                if (module.IsKeyCompetence)
                    return KeyText;

                if (!string.IsNullOrEmpty(module.ExamStatus) &&
                    module.ExamStatus.IndexOf("open", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return OpenText;
                }

                return NoGradeFallback;
            }

            if (value is float f)
            {
                if (Math.Abs(f - NotEvaluatedGrade) < Epsilon) return NotEvaluatedText;
                return f.ToString("F1", CultureInfo.InvariantCulture).Replace(".", ",");
            }

            if (value is double d)
            {
                var parsed = (float)d;
                if (Math.Abs(parsed - NotEvaluatedGrade) < Epsilon) return NotEvaluatedText;
                return parsed.ToString("F1", CultureInfo.InvariantCulture).Replace(".", ",");
            }

            if (value == null) return NoGradeFallback;

            try
            {
                var s = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                var parsed = (float)s;
                if (Math.Abs(parsed - NotEvaluatedGrade) < Epsilon) return NotEvaluatedText;
                return parsed.ToString("F1", CultureInfo.InvariantCulture).Replace(".", ",");
            }
            catch
            {
                return NoGradeFallback;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}