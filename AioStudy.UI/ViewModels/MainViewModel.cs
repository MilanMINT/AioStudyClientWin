using AioStudy.Core.Data.Services;
using AioStudy.Core.Manager.Settings;
using AioStudy.Core.Services;
using AioStudy.Core.Util;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels.Components;
using AioStudy.UI.ViewModels.Forms;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace AioStudy.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITimerService _timerService;
        private readonly UserDbService _userDbService;
        private readonly SemesterDbService _semesterDbService;

        private ViewModelBase _currentViewModel;
        private DashboardViewModel _dashboardViewModel;
        private PomodoroViewModel _pomodoroViewModel;
        private SettingsViewModel _settingsViewModel;
        private GradesViewModel _gradesViewModel;
        private SemesterViewModel _semesterViewModel;
        private ModulesViewModel _modulesViewModel;

        private TimerOverlayViewModel _timerOverlayViewModel;
        private bool _isMainWindowMinimized;

        private string _currentViewName;

        public RelayCommand Dark { get; }
        public RelayCommand Light { get; }
        public RelayCommand ShowDashboardCMD { get; }
        public RelayCommand ShowSettingsCMD { get; }
        public RelayCommand ShowSemesterCMD { get; }
        public RelayCommand ShowPomodoroCMD { get; }
        public RelayCommand ShowGradesCMD { get; }
        public RelayCommand ShowModulesCMD { get; }

        private TimeSpan _time;

        private bool _showStatusBar;
        private bool _isRunning;
        private bool _isPaused;

        // FirstSecondLetterOfUsername
        //Username
        //CurrentSemester

        private string _firstSecondLetterOfUsername;
        private string _username;
        private string _currentSemester;

        public string FirstSecondLetterOfUsername
        {
            get => _firstSecondLetterOfUsername;
            set
            {
                _firstSecondLetterOfUsername = value;
                OnPropertyChanged(nameof(FirstSecondLetterOfUsername));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string CurrentSemester
        {
            get => _currentSemester;
            set
            {
                _currentSemester = value;
                OnPropertyChanged(nameof(CurrentSemester));
            }
        }

        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set
            {
                _showStatusBar = value;
                OnPropertyChanged(nameof(ShowStatusBar));
            }
        }

        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
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

        public TimerOverlayViewModel TimerOverlayViewModel
        {
            get => _timerOverlayViewModel;
            set
            {
                _timerOverlayViewModel = value;
                OnPropertyChanged(nameof(TimerOverlayViewModel));
            }
        }

        public bool IsMainWindowMinimized
        {
            get => _isMainWindowMinimized;
            set
            {
                _isMainWindowMinimized = value;
                OnPropertyChanged(nameof(IsMainWindowMinimized));
                UpdateTimerOverlayVisibility();
            }
        }

        public MainViewModel(ITimerService timerService, UserDbService userDbService, SemesterDbService semesterDbService)
        {
            _timerService = timerService;
            _userDbService = userDbService;
            _semesterDbService = semesterDbService;

            // Commands initialisieren
            Dark = new RelayCommand(ExecuteDarkCommand);
            Light = new RelayCommand(ExecuteLightCommand);
            ShowDashboardCMD = new RelayCommand(ExecuteShowDashboardCommand);
            ShowSettingsCMD = new RelayCommand(ExecuteShowSettingsCommand);
            ShowSemesterCMD = new RelayCommand(ExecuteShowSemesterCommand);
            ShowPomodoroCMD = new RelayCommand(ExecuteShowPomodoroCommand);
            ShowGradesCMD = new RelayCommand(ExecuteShowGradesCommand);
            ShowModulesCMD = new RelayCommand(ExecuteShowModulesCommand);

            TimerOverlayViewModel = App.ServiceProvider.GetRequiredService<TimerOverlayViewModel>();

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

            _pomodoroViewModel.PropertyChanged += OnPomodoroViewModelPropertyChanged;

            CheckForExistingUser();
            LoadUserBottomInfoPanel();

            _timerService.TimeChanged += TimerService_TimeChanged;
            _timerService.RunningStateChanged += TimerService_RunningStateChanged;
            _timerService.PausedStateChanged += TimerService_PausedStateChanged;
            _timerService.TimerEnded += OnTimerCompleted;
        }

        private void OnTimerCompleted(object? sender, EventArgs e)
        {
            Application.Current?.Dispatcher.Invoke(async () =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    if (mainWindow.WindowState == WindowState.Minimized)
                    {
                        CurrentViewModel = _pomodoroViewModel;
                        CurrentViewName = "Pomodoro";
                        mainWindow.WindowState = WindowState.Normal;
                    }
                    

                    mainWindow.Activate();
                    mainWindow.Topmost = true;
                    mainWindow.Topmost = false;

                    SystemSounds.Exclamation.Play();

                    FlashWindow(mainWindow);
                    await ToastService.ShowSuccessAsync("Pomodoro Timer", "Your Pomodoro session has ended!");

                }
            });
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        private void FlashWindow(Window window)
        {
            var helper = new WindowInteropHelper(window);
            FlashWindow(helper.Handle, true);
        }

        private void OnPomodoroViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PomodoroViewModel.IsRunning) ||
                e.PropertyName == nameof(PomodoroViewModel.IsPaused))
            {
                UpdateTimerOverlayVisibility();
            }
        }

        private void UpdateTimerOverlayVisibility()
        {
            if (TimerOverlayViewModel != null && _pomodoroViewModel != null)
            {
                TimerOverlayViewModel.IsVisible = IsMainWindowMinimized && _pomodoroViewModel.IsRunning;
            }
        }

        private void TimerService_TimeChanged(object? sender, TimeSpan e)
        {
            Time = e;
        }

        private void TimerService_PausedStateChanged(object? sender, bool e)
        {
            _isPaused = e;
            UpdateTimerStatusBarVisibility();
        }

        private void TimerService_RunningStateChanged(object? sender, bool e)
        {
            _isRunning = e;
            UpdateTimerStatusBarVisibility();
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

        public void Dispose()
        {
            (_pomodoroViewModel as IDisposable)?.Dispose();
            (_gradesViewModel as IDisposable)?.Dispose();
            _timerService.RunningStateChanged -= TimerService_RunningStateChanged;
            _timerService.PausedStateChanged -= TimerService_PausedStateChanged;
            _timerService.TimeChanged -= TimerService_TimeChanged;
            // REST KOMMT NOCH
        }

        private void UpdateTimerStatusBarVisibility()
        {
            ShowStatusBar = _isRunning;
        }

        public async void LoadUserBottomInfoPanel()
        {
            var user = await _userDbService.GetUser()!;
            var firstSecondLetter = StringUtils.GetFirstSecondLetter(user?.Username);
            FirstSecondLetterOfUsername = firstSecondLetter?.ToUpper() ?? "??";
            CurrentSemester = user?.CurrentSemester?.Name ?? "Current semester not set!";
            Username = user?.Username ?? "Username not set!";
        }
    }
}
