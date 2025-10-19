using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AioStudy.Core.Services;

namespace AioStudy.UI.WpfServices
{
    public class WpfThemeService : IThemeService
    {
        public void ApplyTheme(string themeName)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            var uri = new Uri($"Themes/{themeName}Theme.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        }
    }
}
