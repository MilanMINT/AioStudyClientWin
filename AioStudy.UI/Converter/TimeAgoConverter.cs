using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class TimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var timeSpan = DateTime.Now - dateTime;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} min ago";
                if (timeSpan.TotalHours < 24)
                    return timeSpan.TotalHours < 2
                        ? "1 hour ago"
                        : $"{(int)timeSpan.TotalHours} hours ago";
                if (timeSpan.TotalDays < 7)
                    return timeSpan.TotalDays < 2
                        ? "1 day ago"
                        : $"{(int)timeSpan.TotalDays} days ago";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)} weeks ago";

                return dateTime.ToString("dd.MM.yyyy");
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
