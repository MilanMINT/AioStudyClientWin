using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Manager.Settings
{
    public class AppSettings
    {
        public string Theme { get; set; } = "Dark";

        public AppSettings(){}

        public AppSettings(AppSettings appSettings)
        {
            Theme = appSettings.Theme;
        }
    }
}
