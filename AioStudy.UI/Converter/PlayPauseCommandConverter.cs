using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace AioStudy.UI.Converter
{
    public class PlayPauseCommandConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 5 || !(values[3] is bool) || !(values[4] is TimeSpan))
                return null;

            var startCommand = values[0] as ICommand;
            var pauseCommand = values[1] as ICommand;
            var resumeCommand = values[2] as ICommand;
            var isRunning = (bool)values[3];
            var remaining = (TimeSpan)values[4];

            if (isRunning)
            {
                // Timer läuft → Pause
                return pauseCommand;
            }
            else if (remaining > TimeSpan.Zero)
            {
                // Timer ist pausiert (hat noch Zeit übrig) → Resume
                return resumeCommand;
            }
            else
            {
                // Timer ist bei 0 → Start (neuer Timer)
                return startCommand;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
