using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Core.Util;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels.Components;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AioStudy.UI.ViewModels
{
    public class PomodoroViewModel : ViewModelBase, IDisposable
    {
        private string _text = string.Empty;
        private readonly ITimerService _timerService;
        private TimeSpan _remaining;
        private DateTime _timerFinishedTime;
        private readonly DispatcherTimer _clockTimer;
        private QuickTimersViewModel _quickTimersViewModel;
        private Module _selectedModule;
        private string _activeBreakButton = "";
        private int _minutes = 25;
        private int _seconds = 0;
        private bool _isPaused;
        private bool _isRunning;
        private bool _canChangeTime;
        private bool _canChangeModule;
        private double _gradientAnimationDuration = 1.0;
        private string _gradientColor1 = "#3A3D45"; // Blau 1
        private string _gradientColor2 = "#3A3D45"; // Blau 2
        private string _gradientColor3 = "#a5bacc"; // Blau 3
        private bool _isBreakActive = false;

        public double GradientAnimationDuration
        {
            get => _gradientAnimationDuration;
            set
            {
                _gradientAnimationDuration = value;
                OnPropertyChanged(nameof(GradientAnimationDuration));
            }
        }

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

        private ObservableCollection<Module> _modules = new ObservableCollection<Module>();

        private readonly ModulesDbService _modulesDbService;

        public string ActiveBreakButton
        {
            get { return _activeBreakButton; }
            set
            {
                _activeBreakButton = value;
                OnPropertyChanged(nameof(ActiveBreakButton));
                OnPropertyChanged(nameof(IsShortBreakActive));
                OnPropertyChanged(nameof(IsMidBreakActive));
                OnPropertyChanged(nameof(IsLongBreakActive));
                OnPropertyChanged(nameof(IsBreakActive));
                ControlTimerCommand?.RaiseCanExecuteChanged();

            }
        }

        public string IsShortBreakActive => ActiveBreakButton == "Short" ? "Active" : "";
        public string IsMidBreakActive => ActiveBreakButton == "Mid" ? "Active" : "";
        public string IsLongBreakActive => ActiveBreakButton == "Long" ? "Active" : "";
        public bool IsBreakActive
        {
            get {  return !string.IsNullOrEmpty(ActiveBreakButton); }
            set
            {
                _isBreakActive = value;
                OnPropertyChanged(nameof(IsBreakActive));
            }

        }

        public QuickTimersViewModel QuickTimersViewModel
        {
            get => _quickTimersViewModel;
            set
            {
                _quickTimersViewModel = value;
                OnPropertyChanged(nameof(QuickTimersViewModel));
            }
        }

        public bool CanChangeModule
        {
            get { return _canChangeModule; }
            set
            {
                _canChangeModule = value;
                OnPropertyChanged(nameof(CanChangeModule));
            }
        }

        public ObservableCollection<Module> Modules
        {
            get { return _modules; }
            set
            {
                _modules = value;
                OnPropertyChanged(nameof(Modules));
            }
        }

        public Module SelectedModule
        {
            get { return _selectedModule; }
            set
            {
                _selectedModule = value;
                OnPropertyChanged(nameof(SelectedModule));
            }
        }

        public DateTime TimerFinishedTime
        {
            get { return _timerFinishedTime; }
            set
            {
                _timerFinishedTime = value;
                OnPropertyChanged(nameof(TimerFinishedTime));
            }
        }

        public bool CanChangeTime
        {
            get { return _canChangeTime; }
            set
            {
                _canChangeTime = value;
                OnPropertyChanged(nameof(CanChangeTime));
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }

        public TimeSpan Remaining
        {
            get { return _remaining; }
            set
            {
                _remaining = value;
                OnPropertyChanged(nameof(Remaining));

                _minutes = (int)_remaining.TotalMinutes;
                _seconds = _remaining.Seconds;

                OnPropertyChanged(nameof(Minutes));
                OnPropertyChanged(nameof(Seconds));
            }
        }

        public int Minutes
        {
            get { return _minutes; }
            set
            {
            _minutes = value;
            OnPropertyChanged(nameof(Minutes));
            OnPropertyChanged(nameof(TimerFinishedTime));
            UpdateTimerFinishedTime();
            ControlTimerCommand?.RaiseCanExecuteChanged();
            }
        }

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                OnPropertyChanged(nameof(Seconds));
                OnPropertyChanged(nameof(TimerFinishedTime));
                UpdateTimerFinishedTime();
                ControlTimerCommand?.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand StartTimerCommand { get; }
        public RelayCommand PauseTimerCommand { get; }
        public RelayCommand ResumeTimerCommand { get; }
        public RelayCommand ResetTimerCommand { get; }
        public RelayCommand ClearSelectionCommand { get; }
        public RelayCommand ControlTimerCommand { get; }
        public RelayCommand EndBreakCommand { get; }
        public RelayCommand StartShortBreakCommand { get; }
        public RelayCommand StartMidBreakCommand { get; }
        public RelayCommand StartLongBreakCommand { get; }

        private MainViewModel _mainViewModel;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        WaveOutEvent output;
        AudioFileReader reader;

        public PomodoroViewModel(ITimerService timerService, ModulesDbService modulesDbService, QuickTimersViewModel quickTimersViewModel)
        {
            Text = "Initial Text";
            _timerService = timerService;
            _modulesDbService = modulesDbService;
            QuickTimersViewModel = quickTimersViewModel;

            IsPaused = false;
            IsRunning = false;
            CanChangeTime = true;
            SelectedModule = null;
            CanChangeModule = true;

            _modules = new ObservableCollection<Module>();

            QuickTimersViewModel.QuickTimerSelected += OnQuickTimerSelected;

            StartTimerCommand = new RelayCommand(StartTimer);
            PauseTimerCommand = new RelayCommand(PauseTimer);
            ResumeTimerCommand = new RelayCommand(ResumeTimer);
            ResetTimerCommand = new RelayCommand(ResetTimer);
            ControlTimerCommand = new RelayCommand(ControlTimer, CanStartTimer);

            EndBreakCommand = new RelayCommand(EndBreak);

            StartShortBreakCommand = new RelayCommand(StartShortBreak, CanStartBreak);
            StartMidBreakCommand = new RelayCommand(StartMidBreak, CanStartBreak);
            StartLongBreakCommand = new RelayCommand(StartLongBreak, CanStartBreak);

            ClearSelectionCommand = new RelayCommand(_ => SelectedModule = null);

            UpdateTimerFinishedTime();

            // Events
            _timerService.TimeChanged += OnTimeChanged;
            _timerService.PausedStateChanged += OnPausedStateChanged;
            _timerService.RunningStateChanged += OnRunningStateChanged;
            _timerService.BreakEnded += OnBreakEnded;
            _timerService.BreakStateChanged += OnBreakStateChanged;
            _timerService.Last10Seconds += OnLast10Seconds;

            // Clock Timer
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += ClockTimer_Tick;
            _clockTimer.Start();

            _ = LoadModulesAsync();
        }

        private void OnLast10Seconds(object? sender, EventArgs e)
        {
            GradientAnimationDuration = 0.15;

            if (Remaining.TotalSeconds % 4 == 0)
            {
                AnimateGradientToGreen();
            }
            else if (Remaining.TotalSeconds % 4 == 1)
            {
                AnimateGradientToBlue();
            }
            else if (Remaining.TotalSeconds % 4 == 2)
            {
                AnimateGradientToRed();
            }
            else
            {
                AnimateGradientToBlue();
            }
        }

        private void OnBreakStateChanged(object? sender, Enums.TimerBreakType e)
        {
            switch (e)
            {
                case Enums.TimerBreakType.Short:
                    AnimateGradientToYellow();
                    break;
                case Enums.TimerBreakType.Mid:
                    AnimateGradientToOrange();
                    break;
                case Enums.TimerBreakType.Long:
                    AnimateGradientToGreen();
                    break;
                default:
                    AnimateGradientToBlue();
                    break;
            }
        }
        private void AnimateGradientToRed()
        {
            GradientColor1 = "#FF6B6B"; // Rot 1
            GradientColor2 = "#C92A2A"; // Rot 2
            GradientColor3 = "#FF8787"; // Rot 3
        }

        private void AnimateGradientToYellow()
        {
            GradientColor1 = "#FFEB3B"; // Gelb 1
            GradientColor2 = "#FBC02D"; // Gelb 2
            GradientColor3 = "#FFF176"; // Gelb 3
        }

        private void AnimateGradientToOrange()
        {
            GradientColor1 = "#FFA726"; // Orange 1
            GradientColor2 = "#FB8C00"; // Orange 2
            GradientColor3 = "#FFB74D"; // Orange 3
        }

        private void AnimateGradientToGreen()
        {
            GradientColor1 = "#66BB6A"; // Grün 1
            GradientColor2 = "#43A047"; // Grün 2
            GradientColor3 = "#81C784"; // Grün 3
        }

        private void AnimateGradientToBlue()
        {
            GradientColor1 = "#608BC1"; // Blau 1
            GradientColor2 = "#133E87"; // Blau 2
            GradientColor3 = "#a5bacc"; // Blau 3
        }

        private void AnimateGradientToStopped()
        {
            GradientColor1 = "#3A3D45"; // Grey 1
            GradientColor2 = "#3A3D45"; // Grey 2
            GradientColor3 = "#a5bacc"; // Grey 3
        }

        private void OnBreakEnded(object? sender, EventArgs e)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                ActiveBreakButton = "";
                AnimateGradientToBlue();
                ControlTimerCommand?.RaiseCanExecuteChanged();
            });
        }

        private bool CanStartBreak(object? arg)
        {
            if(!_isRunning)
            {
                return false;
            }
            return true;
        }

        private void StartShortBreak(object? obj)
        {
            ActiveBreakButton = "Short";
            _timerService.StartBreak(Enums.TimerBreakType.Short);
        }

        private void StartMidBreak(object? obj)
        {
            ActiveBreakButton = "Mid";
            _timerService.StartBreak(Enums.TimerBreakType.Mid);
        }

        private void StartLongBreak(object? obj)
        {
            ActiveBreakButton = "Long";
            _timerService.StartBreak(Enums.TimerBreakType.Long);
        }

        private void EndBreak(object? obj)
        {
            _timerService.EndBreak();
            ActiveBreakButton = "";
            IsBreakActive = false;
            ControlTimerCommand?.RaiseCanExecuteChanged();  
        }

        private void OnQuickTimerSelected(object? sender, QuickTimer qt)
        {
            if (_isRunning)
            {
                return;
            }
            Minutes = qt.Duration.Minutes;
            Seconds = qt.Duration.Seconds;
            SelectedModule = qt.Module;

            if (CanStartTimer(null))
            {
                StartTimer(null);
            }
        }

        public async Task LoadModulesAsync()
        {
            Modules.Clear();
            var modules = await _modulesDbService.GetAllModulesAsync();
            foreach (var module in modules)
            {
                Modules.Add(module);
            }
        }

        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isRunning || _isPaused)
            {
                UpdateTimerFinishedTime();
            }
        }

        private void OnTimeChanged(object? sender, TimeSpan time)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Remaining = time;
            });
        }

        private void OnPausedStateChanged(object? sender, bool isPaused)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                IsPaused = isPaused;
            });
        }

        private void OnRunningStateChanged(object? sender, bool isRunning)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                IsRunning = isRunning;
                CanChangeTime = !isRunning;

                if (!isRunning)
                {
                    _minutes = (int)_remaining.TotalMinutes;
                    _seconds = _remaining.Seconds;
                    OnPropertyChanged(nameof(Minutes));
                    OnPropertyChanged(nameof(Seconds));

                    CanChangeModule = true;
                    ControlTimerCommand?.RaiseCanExecuteChanged();
                    ActiveBreakButton = "";
                    AnimateGradientToStopped();
                    GradientAnimationDuration = 1.0;
                }
            });
        }

        private void ControlTimer(object? obj)
        {
            if (_timerService.IsRunning)
            {
                if (_timerService.IsPaused)
                {
                    ResumeTimer(null);
                }
                else
                {
                    PauseTimer(null);
                }
            }
            else
            {
                StartTimer(null);
            }
        }

        private bool CanStartTimer(object? arg)
        {
            if (_minutes == 0 && _seconds == 0 || IsBreakActive)
            {
                return false;
            }
            return true;
        }

        private void ResumeTimer(object? obj)
        {
            _timerService.Resume();
            CanChangeModule = false;
            AnimateGradientToBlue();
        }

        private void PauseTimer(object? obj)
        {
            _timerService.Pause();
            CanChangeModule = false;
            AnimateGradientToRed();
        }

        private void StartTimer(object? obj)
        {
            int totalSeconds = (_minutes * 60) + _seconds;
            _timerService.Start(TimeSpan.FromSeconds(totalSeconds), _selectedModule);
            CanChangeModule = false;
            GradientAnimationDuration = 1.0;
            AnimateGradientToBlue();

        }

        private void ResetTimer(object? obj)
        {
            _timerService.Reset();
            CanChangeModule = true;
            AnimateGradientToStopped();
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        private void UpdateTimerFinishedTime()
        {
            TimerFinishedTime = DateTime.Now.AddMinutes(Minutes).AddSeconds(Seconds);
        }

        public void Dispose()
        {
            if (_clockTimer != null)
            {
                _clockTimer.Stop();
                _clockTimer.Tick -= ClockTimer_Tick;
            }

            if (QuickTimersViewModel != null)
            {
                QuickTimersViewModel.QuickTimerSelected -= OnQuickTimerSelected;
            }

            _timerService.TimeChanged -= OnTimeChanged;
            _timerService.PausedStateChanged -= OnPausedStateChanged;
            _timerService.RunningStateChanged -= OnRunningStateChanged;
            _timerService.BreakEnded -= OnBreakEnded;
        }
    }
}