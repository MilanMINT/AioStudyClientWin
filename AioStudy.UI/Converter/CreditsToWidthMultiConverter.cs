using System;
using System.Globalization;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class CreditsToWidthMultiConverter : IMultiValueConverter
    {
        //[0]=moduleCredits, [1]=totalCredits, [2]=modulesContainerWidth [3]=marginCompensation
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int extra = 0;
                if (parameter != null && int.TryParse(parameter.ToString(), out var p)) extra = p;

                if (values.Length != 4 ||
                    values[0] is not int moduleCredits ||
                    values[1] is not int totalCredits ||
                    values[2] is not int containerWidth ||
                    values[3] is not int modulesCount ||
                    totalCredits <= 0)
                {
                    return 0;
                }

                if (modulesCount > 0)
                {
                    containerWidth -= extra * (modulesCount - 1);
                }

                double width = (double)moduleCredits / totalCredits * containerWidth;
                return Math.Max(width, 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}