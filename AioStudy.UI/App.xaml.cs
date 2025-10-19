using AioStudy.Core.Manager;
using AioStudy.Core.Manager.Settings;
using AioStudy.UI.WpfServices;
using System.Configuration;
using System.Data;
using System.Windows;

namespace AioStudy.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            base.OnStartup(e);

            var themeService = new WpfThemeService();
            var settingsManager = SettingsManager.Instance;
            settingsManager.SetThemeService(themeService);
            settingsManager.Init();

            bool res = DbManager.InitializeDatabase();
            if (!res)
            {
                MessageBox.Show("Datenbankinitialisierung fehlgeschlagen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
