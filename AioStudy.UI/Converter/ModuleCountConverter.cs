using AioStudy.Core.Data.Services;
using AioStudy.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AioStudy.UI.Converter
{
    public class ModuleCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Semester semester)
            {
                try
                {
                    var semesterService = App.ServiceProvider.GetRequiredService<SemesterDbService>();
                    var modules = semesterService.GetModulesCountForSemester(semester).Result;
                    return $"{modules} Module(s)";
                }
                catch
                {
                    return "0 Module(s)";
                }
            }
            return "0 Module(s)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
