using AioStudy.Core.Data.Services;
using AioStudy.Core.Manager;
using AioStudy.Core.Manager.Settings;
using AioStudy.Data.EF;
using AioStudy.Data.Interfaces;
using AioStudy.Data.Services;
using AioStudy.Models;
using AioStudy.UI.ViewModels;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
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
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

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

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<AppDbContext>();

            //Repositories
            services.AddTransient<IRepository<User>, Repository<User>>();
            services.AddTransient<IRepository<Module>, Repository<Module>>();
            services.AddTransient<IRepository<Semester>, Repository<Semester>>();
            services.AddTransient<IRepository<DailyModuleStats>, Repository<DailyModuleStats>>();
            services.AddTransient<IRepository<LearnSession>, Repository<LearnSession>>();

            //Services
            services.AddTransient<SemesterDbService>();
            services.AddTransient<ModulesDbService>();

            //ViewModels
            services.AddTransient<SemesterViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ModulesViewModel>();
            services.AddTransient<MainViewModel>();

            //WPF Services
            services.AddSingleton<WpfThemeService>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
