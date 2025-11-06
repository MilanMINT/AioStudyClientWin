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
            if (values.Length != 3 || !(values[2] is bool))
                return null;

            var startCommand = values[0] as ICommand;
            var pauseCommand = values[1] as ICommand;
            var isRunning = (bool)values[2];

            return isRunning ? pauseCommand : startCommand;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
