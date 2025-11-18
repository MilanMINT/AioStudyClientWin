using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NAudio.Wave;
using System.Windows.Threading;
using System.IO;

namespace AioStudy.UI.ViewModels
{
    public class PomodoroViewModel : ViewModelBase, IDisposable
    {
        private string _text = string.Empty;
        private readonly ITimerService _timerService;
        private TimeSpan _remaining;
        private DateTime _timerFinishedTime;
        private readonly DispatcherTimer _clockTimer;
        private Module _selectedModule;
        private int _minutes = 25;
        private int _seconds = 0;
        private bool _isPaused;
        private bool _isRunning;
        private bool _canChangeTime;

        private ObservableCollection<Module> _modules = new ObservableCollection<Module>();

        private readonly ModulesDbService _modulesDbService;

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

                Minutes = (int)_remaining.TotalMinutes;
                Seconds = _remaining.Seconds;
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

        public PomodoroViewModel(ITimerService timerService, ModulesDbService modulesDbService)
        {
            Text = "Initial Text";
            _timerService = timerService;
            _modulesDbService = modulesDbService;

            IsPaused = false;
            IsRunning = false;
            CanChangeTime = true;
            SelectedModule = null;

            _modules = new ObservableCollection<Module>();

            StartTimerCommand = new RelayCommand(StartTimer);
            PauseTimerCommand = new RelayCommand(PauseTimer);
            ResumeTimerCommand = new RelayCommand(ResumeTimer);
            ResetTimerCommand = new RelayCommand(ResetTimer);
            ControlTimerCommand = new RelayCommand(ControlTimer, CanStartTimer);
            ClearSelectionCommand = new RelayCommand(_ => SelectedModule = null);

            UpdateTimerFinishedTime();

            // Events
            _timerService.TimeChanged += OnTimeChanged;
            _timerService.PausedStateChanged += OnPausedStateChanged;
            _timerService.RunningStateChanged += OnRunningStateChanged;

            // Clock Timer
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += ClockTimer_Tick;
            _clockTimer.Start();

            _ = LoadModulesAsync();
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
            if (_minutes == 0 && _seconds == 0)
                return false;
            return true;
        }

        private void ResumeTimer(object? obj)
        {
            _timerService.Resume();
        }

        private void PauseTimer(object? obj)
        {
            _timerService.Pause();
        }

        private void StartTimer(object? obj)   
        {
            int totalSeconds = (_minutes * 60) + _seconds;
            _timerService.Start(TimeSpan.FromSeconds(totalSeconds), _selectedModule);
        }   

        private void ResetTimer(object? obj)
        {
            _timerService.Reset();
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

            _timerService.TimeChanged -= OnTimeChanged;
            _timerService.PausedStateChanged -= OnPausedStateChanged;
            _timerService.RunningStateChanged -= OnRunningStateChanged;
        }
    }
}