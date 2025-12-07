using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class StringVisibilityConverter : IValueConverter
    {
        public string VisibleValue { get; set; } = "Visible";

        public bool UseHidden { get; set; } = false;

        public bool IsInverted { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return GetHiddenVisibility();

            string stringValue = value.ToString() ?? string.Empty;
            string targetValue = parameter?.ToString() ?? VisibleValue;

            var targetValues = targetValue.Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);

            bool isMatch = false;
            foreach (var target in targetValues)
            {
                if (string.Equals(stringValue, target.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    isMatch = true;
                    break;
                }
            }

            if (IsInverted)
                isMatch = !isMatch;

            return isMatch ? Visibility.Visible : GetHiddenVisibility();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool isVisible = visibility == Visibility.Visible;

                if (IsInverted)
                    isVisible = !isVisible;

                return isVisible ? (parameter?.ToString() ?? VisibleValue) : string.Empty;
            }

            return string.Empty;
        }

        private Visibility GetHiddenVisibility()
        {
            return UseHidden ? Visibility.Hidden : Visibility.Collapsed;
        }
    }
}