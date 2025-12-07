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
        private readonly SettingsManager _settingsManager = SettingsManager.Instance;

        private readonly ITimerService _timerService;
        private readonly UserDbService _userDbService;
        private readonly SemesterDbService _semesterDbService;

        private ViewModelBase _currentViewModel;
        private DashboardViewModel _dashboardViewModel;
        public PomodoroViewModel _pomodoroViewModel;
        private SettingsViewModel _settingsViewModel;
        private GradesViewModel _gradesViewModel;
        private SemesterViewModel _semesterViewModel;
        private ModulesViewModel _modulesViewModel;

        private string _gradientColor1 = "#3A3D45"; // Blau 1
        private string _gradientColor2 = "#3A3D45"; // Blau 2
        private string _gradientColor3 = "#a5bacc"; // Blau 3

        private TimerOverlayViewModel _timerOverlayViewModel;
        private bool _isMainWindowMinimized;

        private string _currentViewName;
        private string _timerStatusText;
        private bool _playSoundOnTimerEnd;

        private double _timerProgress;
        private double _timerMaximum;
        private double _mainTimerMaximum;

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

        public string GradientColor1
        {
            get => _gradientColor1;
            set
            {
                _gradientColor1 = value;
                OnPropertyChanged(nameof(GradientColor1));
            }
        }

        public string GradientColor2
        {
            get => _gradientColor2;
            set
            {
                _gradientColor2 = value;
                OnPropertyChanged(nameof(GradientColor2));
            }
        }

        public string GradientColor3
        {
            get => _gradientColor3;
            set
            {
                _gradientColor3 = value;
                OnPropertyChanged(nameof(GradientColor3));
            }
        }

        public double TimerProgress
        {
            get => _timerProgress;
            set
            {
                _timerProgress = value;
                OnPropertyChanged(nameof(TimerProgress));
            }
        }

        public double TimerMaximum
        {
            get => _timerMaximum;
            set
            {
                _timerMaximum = value;
                OnPropertyChanged(nameof(TimerMaximum));
            }
        }

        public bool PlaySoundOnTimerEnd
        {
            get => _playSoundOnTimerEnd;
            set
            {
                _playSoundOnTimerEnd = value;
                OnPropertyChanged(nameof(PlaySoundOnTimerEnd));
            }
        }

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
        public string TimerStatusText
        {
            get => _timerStatusText;
            private set
            {
                if (_timerStatusText != value)
                {
                    _timerStatusText = value;
                    OnPropertyChanged(nameof(TimerStatusText));
                }
            }
        }


        public bool IsTimerRunning => _timerService.IsRunning;
        public bool IsTimerPaused => _timerService.IsPaused;
        public string? CurrentModuleName => _pomodoroViewModel?.SelectedModule?.Name;

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
            PlaySoundOnTimerEnd = false;

            _pomodoroViewModel.PropertyChanged += OnPomodoroViewModelPropertyChanged;

            CheckForExistingUser();
            LoadUserBottomInfoPanel();

            _timerService.TimeChanged += TimerService_TimeChanged;
            _timerService.RunningStateChanged += TimerService_RunningStateChanged;
            _timerService.PausedStateChanged += TimerService_PausedStateChanged;
            _timerService.TimerEnded += OnTimerCompleted;
            _timerService.BreakStateChanged += OnBreakStateChanged;
            _timerService.BreakEnded += OnBreakEnded;

            ApplyGradientScheme(GradientColorSchemes.TimerBar.Running);
        }

        private void OnBreakEnded(object? sender, EventArgs e)
        {
            TimerMaximum = _mainTimerMaximum;
            TimerProgress = _timerService.Remaining.TotalSeconds;
            TimerStatusText = "Running";
            ApplyGradientScheme(GradientColorSchemes.TimerBar.Running);
        }

        private void OnBreakStateChanged(object? sender, Enums.TimerBreakType e)
        {
            switch (e)
            {
                case Enums.TimerBreakType.Short:
                    TimerMaximum = _settingsManager.Settings.BreakDurationsInMinutes[0] * 60;
                    TimerProgress = _timerService.Remaining.TotalSeconds;
                    TimerStatusText = "On Short Break";
                    ApplyGradientScheme(GradientColorSchemes.TimerBar.ShortBreak);
                    break;
                case Enums.TimerBreakType.Long:
                    TimerMaximum = _settingsManager.Settings.BreakDurationsInMinutes[2] * 60;
                    TimerProgress = _timerService.Remaining.TotalSeconds;
                    TimerStatusText = "On Long Break";
                    ApplyGradientScheme(GradientColorSchemes.TimerBar.LongBreak);
                    break;
                case Enums.TimerBreakType.Mid:
                    TimerMaximum = _settingsManager.Settings.BreakDurationsInMinutes[1] * 60;
                    TimerProgress = _timerService.Remaining.TotalSeconds;
                    TimerStatusText = "On Mid Break";
                    ApplyGradientScheme(GradientColorSchemes.TimerBar.MidBreak);
                    break;
                default:
                    TimerMaximum = _timerService.Remaining.TotalSeconds;
                    TimerProgress = _timerService.Remaining.TotalSeconds;
                    break;
            }
        }

        private void OnTimerCompleted(object? sender, EventArgs e)
        {
            Application.Current?.Dispatcher.Invoke(() =>
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


                    if (PlaySoundOnTimerEnd)
                    {
                        SystemSounds.Exclamation.Play();
                    }
                    FlashWindow(mainWindow);
                }

                return Task.CompletedTask;
            });
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        private void FlashWindow(Window window)
        {
            var helper = new WindowInteropHelper(window);
            FlashWindow(helper.Handle, true);
        }

        private void SetGradientColors(string color1, string color2, string color3)
        {
            GradientColor1 = color1;
            GradientColor2 = color2;
            GradientColor3 = color3;
        }

        private void ApplyGradientScheme(GradientColorSchemes.GradientColors colors)
        {
            GradientColorSchemes.ApplyColors(colors, SetGradientColors);
        }

        private void OnPomodoroViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PomodoroViewModel.IsRunning) ||
                e.PropertyName == nameof(PomodoroViewModel.IsPaused))
            {
                UpdateTimerOverlayVisibility();
            }

            if (e.PropertyName == nameof(PomodoroViewModel.SelectedModule))
            {
                OnPropertyChanged(nameof(CurrentModuleName));
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
            TimerProgress = e.TotalSeconds;
        }

        private void TimerService_PausedStateChanged(object? sender, bool e)
        {
            _isPaused = e;
            UpdateTimerStatusBarVisibility();

            TimerStatusText = e ? "Paused" : "Running";

            if (_timerService.IsPaused)
            {
                ApplyGradientScheme(GradientColorSchemes.TimerBar.Paused);
            }
            else
            {
                ApplyGradientScheme(GradientColorSchemes.TimerBar.Running);
            }
        }

        private void TimerService_RunningStateChanged(object? sender, bool e)
        {
            _isRunning = e;
            UpdateTimerStatusBarVisibility();
            TimerStatusText = e ? "Running" : "Stopped";
            OnPropertyChanged(nameof(IsTimerRunning));
            OnPropertyChanged(nameof(IsTimerPaused));
            OnPropertyChanged(nameof(CurrentModuleName));

            if (e && _timerService.Remaining.TotalSeconds > 0)
            {
                _mainTimerMaximum = _timerService.Remaining.TotalSeconds;
                TimerMaximum = _timerService.Remaining.TotalSeconds;
                TimerProgress = _timerService.Remaining.TotalSeconds;
            }
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
