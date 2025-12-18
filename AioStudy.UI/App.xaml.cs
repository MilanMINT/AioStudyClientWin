using AioStudy.Core.Data.Services;
using AioStudy.Core.Manager;
using AioStudy.Core.Manager.Settings;
using AioStudy.Core.Services;
using AioStudy.Data.EF;
using AioStudy.Data.Interfaces;
using AioStudy.Data.Services;
using AioStudy.Models;
using AioStudy.Models.DailyPlannerModels;
using AioStudy.UI.ViewModels;
using AioStudy.UI.ViewModels.Components;
using AioStudy.UI.ViewModels.Forms;
using AioStudy.UI.ViewModels.Overview;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
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
            LiveCharts.Configure(config => config.AddDarkTheme());

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
                return;
            }

            DailyPlanDbService dailyPlanDbService = ServiceProvider.GetRequiredService<DailyPlanDbService>();
            dailyPlanDbService.InitDailyPlan().Wait();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            //DB
            services.AddTransient<AppDbContext>();

            //Repositories
            services.AddTransient<IRepository<User>, Repository<User>>();
            services.AddTransient<IRepository<Module>, Repository<Module>>();
            services.AddTransient<IRepository<Semester>, Repository<Semester>>();
            services.AddTransient<IRepository<DailyModuleStats>, Repository<DailyModuleStats>>();
            services.AddTransient<IRepository<LearnSession>, Repository<LearnSession>>();
            services.AddTransient<IRepository<QuickTimer>, Repository<QuickTimer>>();
            services.AddTransient<IRepository<DailyPlan>, Repository<DailyPlan>>();
            services.AddTransient<IRepository<DailyTask>, Repository<DailyTask>>();
            services.AddTransient<IRepository<DailySubTask>, Repository<DailySubTask>>();

            //Services
            services.AddTransient<SemesterDbService>();
            services.AddTransient<ModulesDbService>();
            services.AddTransient<UserDbService>();
            services.AddTransient<LearnSessionDbService>();
            services.AddTransient<DailyModuleStatsDbService>();
            services.AddTransient<QuickTimerDbService>();
            services.AddTransient<SettingsManager>();
            services.AddTransient<DailyPlanDbService>();

            services.AddTransient<ModuleOverViewViewModel>();

            //ViewModels
            services.AddSingleton<SemesterViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<GradesViewModel>();
            services.AddSingleton<ModulesViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<PomodoroViewModel>();
            services.AddSingleton<QuickTimersViewModel>();
            services.AddSingleton<TimerOverlayViewModel>();
            services.AddSingleton<DailyPlannerViewModel>();

            services.AddTransient<TimerOverlayViewModel>();
            services.AddTransient<AddSemesterViewModel>();
            services.AddTransient<AddModuleViewModel>();
            services.AddTransient<CreateUsernameViewModel>();

            //WPF Services
            services.AddSingleton<ITimerService, TimerService>();
            services.AddSingleton<WpfThemeService>();
            services.AddSingleton<GlobalHotKeyService>();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (MainWindow?.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
