using AioStudy.Core.Manager.Settings;
using AioStudy.Core.Services;
using AioStudy.Core.Util;
using AioStudy.UI.Commands;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        private readonly ITimerService _timerService;
        private TimeSpan _remaining;
        private bool _isTimerRunning;

        public bool IsTimerRunning
        {
            get
            {
                return _isTimerRunning;
            }
            set
            {
                _isTimerRunning = value;
                OnPropertyChanged(nameof(IsTimerRunning));
            }
        }

        public TimeSpan Remaining
        {
            get { return _remaining; }
            set
            {
                _remaining = value;
                OnPropertyChanged(nameof(Remaining));
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

        public MainViewModel(ITimerService timerService)
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

            // ViewModels von DI bekommen
            _modulesViewModel = App.ServiceProvider.GetRequiredService<ModulesViewModel>();
            _modulesViewModel.SetMainViewModel(this);

            // Direkt instanziierte ViewModels (haben keine DB-Abhängigkeiten)
            _pomodoroViewModel = App.ServiceProvider.GetRequiredService<PomodoroViewModel>();
            _pomodoroViewModel.SetMainViewModel(this);

            _settingsViewModel = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
            _settingsViewModel.SetMainViewModel(this);

            _gradesViewModel = App.ServiceProvider.GetRequiredService<GradesViewModel>();
            _gradesViewModel.SetMainViewModel(this);

            // Standard View setzen
            CurrentViewModel = _dashboardViewModel;
            CurrentViewName = "Dashboard";

            _timerService = timerService;

            IsTimerRunning = _timerService.IsRunning;
            Remaining = TimeSpan.FromSeconds(Math.Ceiling(_timerService.Remaining.TotalSeconds));

            _timerService.TimeChanged += (_, time) =>
            {
                Remaining = TimeSpan.FromSeconds(Math.Ceiling(time.TotalSeconds));
            };
            _timerService.RunningStateChanged += (_, running) => IsTimerRunning = running;
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
    }
}
