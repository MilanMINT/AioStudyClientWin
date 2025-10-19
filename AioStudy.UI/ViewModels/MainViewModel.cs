using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AioStudy.UI.Commands;
using AioStudy.Core.Manager.Settings;
using AioStudy.Core.Util;
using Microsoft.Extensions.Primitives;

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

        private string _currentViewName;
        
        public RelayCommand Dark { get; }
        public RelayCommand Light { get; }
        public RelayCommand ShowDashboardCMD { get; }
        public RelayCommand ShowSettingsCMD { get; }
        public RelayCommand ShowSemesterCMD { get; }
        public RelayCommand ShowPomodoroCMD { get; }
        public RelayCommand ShowGradesCMD { get; }

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
            Dark = new RelayCommand(ExecuteDarkCommand);
            Light = new RelayCommand(ExecuteLightCommand);
            ShowDashboardCMD = new RelayCommand(ExecuteShowDashboardCommand);
            ShowSettingsCMD = new RelayCommand(ExecuteShowSettingsCommand);
            ShowSemesterCMD = new RelayCommand(ExecuteShowSemesterCommand);
            ShowPomodoroCMD = new RelayCommand(ExecuteShowPomodoroCommand);
            ShowGradesCMD = new RelayCommand(ExecuteShowGradesCommand);

            // Alle ViewModels einmal erstellen
            _dashboardViewModel = new DashboardViewModel(this);
            _pomodoroViewModel = new PomodoroViewModel(this);
            _semesterViewModel = new SemesterViewModel(this);
            _settingsViewModel = new SettingsViewModel(this);
            _gradesViewModel = new GradesViewModel(this);

            CurrentViewModel = _dashboardViewModel;
            CurrentViewName = "Dashboard";
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
