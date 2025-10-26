using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class StringToEnableConverter : IValueConverter
    {
        /// <summary>
        /// Der String-Wert, bei dem das Element enabled sein soll
        /// </summary>
        public string EnabledValue { get; set; } = "Enabled";

        /// <summary>
        /// Ob die Logik umgekehrt werden soll (invertiert)
        /// </summary>
        public bool IsInverted { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return !IsInverted;

            string stringValue = value.ToString() ?? string.Empty;
            string targetValue = parameter?.ToString() ?? EnabledValue;

            bool isMatch = string.Equals(stringValue, targetValue, StringComparison.OrdinalIgnoreCase);

            if (IsInverted)
                isMatch = !isMatch;

            return isMatch;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled)
            {
                if (IsInverted)
                    isEnabled = !isEnabled;

                return isEnabled ? (parameter?.ToString() ?? EnabledValue) : string.Empty;
            }

            return string.Empty;
        }
    }
}
