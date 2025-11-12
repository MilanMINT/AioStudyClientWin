using AioStudy.Core.Data.Services;
using AioStudy.Core.Manager.Settings;
using AioStudy.Core.Services;
using AioStudy.Core.Util;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels.Forms;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AioStudy.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private DashboardViewModel _dashboardViewModel;
        private PomodoroViewModel _pomodoroViewModel;
        private SettingsViewModel _settingsViewModel;
        private GradesViewModel _gradesViewModel;
        private SemesterViewModel _semesterViewModel;
        private ModulesViewModel _modulesViewModel;

        private string _currentViewName;

        public RelayCommand Dark { get; }
        public RelayCommand Light { get; }
        public RelayCommand ShowDashboardCMD { get; }
        public RelayCommand ShowSettingsCMD { get; }
        public RelayCommand ShowSemesterCMD { get; }
        public RelayCommand ShowPomodoroCMD { get; }
        public RelayCommand ShowGradesCMD { get; }
        public RelayCommand ShowModulesCMD { get; }

        private bool _showStatusBar = true;
        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set
            {
                _showStatusBar = value;
                OnPropertyChanged(nameof(ShowStatusBar));
            }
        }

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public string CurrentViewName
        {
            get { return _currentViewName; }
            set
            {
                _currentViewName = value;
                OnPropertyChanged(nameof(CurrentViewName));
            }
        }

        public MainViewModel()
        {
            // Commands initialisieren
            Dark = new RelayCommand(ExecuteDarkCommand);
            Light = new RelayCommand(ExecuteLightCommand);
            ShowDashboardCMD = new RelayCommand(ExecuteShowDashboardCommand);
            ShowSettingsCMD = new RelayCommand(ExecuteShowSettingsCommand);
            ShowSemesterCMD = new RelayCommand(ExecuteShowSemesterCommand);
            ShowPomodoroCMD = new RelayCommand(ExecuteShowPomodoroCommand);
            ShowGradesCMD = new RelayCommand(ExecuteShowGradesCommand);
            ShowModulesCMD = new RelayCommand(ExecuteShowModulesCommand);

            _semesterViewModel = App.ServiceProvider.GetRequiredService<SemesterViewModel>();
            _semesterViewModel.SetMainViewModel(this);

            _dashboardViewModel = App.ServiceProvider.GetRequiredService<DashboardViewModel>();
            _dashboardViewModel.SetMainViewModel(this);

            _modulesViewModel = App.ServiceProvider.GetRequiredService<ModulesViewModel>();
            _modulesViewModel.SetMainViewModel(this);

            _pomodoroViewModel = App.ServiceProvider.GetRequiredService<PomodoroViewModel>();
            _pomodoroViewModel.SetMainViewModel(this);

            _settingsViewModel = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
            _settingsViewModel.SetMainViewModel(this);

            _gradesViewModel = App.ServiceProvider.GetRequiredService<GradesViewModel>();
            _gradesViewModel.SetMainViewModel(this);

            CurrentViewModel = _dashboardViewModel;
            CurrentViewName = "Dashboard";

            ShowStatusBar = false;

            CheckForExistingUser();
        }

        private void ExecuteShowModulesCommand(object? obj)
        {
            CurrentViewModel = _modulesViewModel;
            CurrentViewName = "Modules";
        }

        private void ExecuteShowGradesCommand(object? obj)
        {
            CurrentViewModel = _gradesViewModel;
            CurrentViewName = "Grades";
        }

        private void ExecuteShowPomodoroCommand(object? obj)
        {
            CurrentViewModel = _pomodoroViewModel;
            CurrentViewName = "Pomodoro";
        }

        private void ExecuteShowSemesterCommand(object? obj)
        {
            CurrentViewModel = _semesterViewModel;
            CurrentViewName = "Semester";
        }

        private void ExecuteShowSettingsCommand(object? obj)
        {
            CurrentViewModel = _settingsViewModel;
            CurrentViewName = "Settings";
        }

        private void ExecuteShowDashboardCommand(object? obj)
        {
            CurrentViewModel = _dashboardViewModel;
            CurrentViewName = "Dashboard";
        }

        private void ExecuteDarkCommand(object? parameter)
        {
            var settingsManager = SettingsManager.Instance;

            settingsManager.Settings.Theme = Enums.ApplicationTheme.Dark.ToString();

            settingsManager.SaveSettings();
        }

        private void ExecuteLightCommand(object? parameter)
        {
            var settingsManager = SettingsManager.Instance;

            settingsManager.Settings.Theme = Enums.ApplicationTheme.Light.ToString();

            settingsManager.SaveSettings();
        }

        private async void CheckForExistingUser()
        {
            bool hasUser;
            try
            {
                hasUser = await App.ServiceProvider.GetRequiredService<UserDbService>().IsUserTableEmpty() == false;
            }
            catch (Exception)
            {
                hasUser = false;
            }
            if (!hasUser)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var vm = App.ServiceProvider.GetRequiredService<CreateUsernameViewModel>();
                    var createUsernameView = new CreateUsername
                    {
                        DataContext = vm,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsLoaded)
                    {
                        createUsernameView.Owner = Application.Current.MainWindow;
                        createUsernameView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    }

                    bool? result = createUsernameView.ShowDialog();
                    if (!result.HasValue || result.Value == false)
                    {
                        Application.Current.Shutdown();
                    }
                });
            }
        }
    }
}
