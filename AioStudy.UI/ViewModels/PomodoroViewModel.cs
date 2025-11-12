using AioStudy.Core.Services;
using AioStudy.UI.Commands;
using System;
using System.Windows;
using System.Windows.Input;

namespace AioStudy.UI.ViewModels
{
    public class PomodoroViewModel : ViewModelBase
    {
        private string _text = string.Empty;
        private readonly ITimerService _timerService;
        private TimeSpan _remaining;
        private int _minutes = 25;
        private int _seconds = 0;
        private bool _isPaused;
        private bool _isRunning;

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
            }
        }

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                OnPropertyChanged(nameof(Seconds));
            }
        }

        public RelayCommand StartTimerCommand { get; }
        public RelayCommand PauseTimerCommand { get; }
        public RelayCommand ResumeTimerCommand { get; }
        public RelayCommand ResetTimerCommand { get; }

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

        public PomodoroViewModel(ITimerService timerService)
        {
            Text = "Initial Text";
            _timerService = timerService;

            StartTimerCommand = new RelayCommand(StartTimer, CanStartTimer);
            PauseTimerCommand = new RelayCommand(PauseTimer);
            ResumeTimerCommand = new RelayCommand(ResumeTimer);
            ResetTimerCommand = new RelayCommand(ResetTimer);
            ControlTimerCommand = new RelayCommand(ControlTimer);

            _timerService.TimeChanged += (s, time) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Remaining = time;
                });
            };

            _timerService.PausedStateChanged += (s, isPaused) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsPaused = isPaused;
                });
            };

            _timerService.RunningStateChanged += (s, isRunning) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsRunning = isRunning;
                });
            };
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
            _timerService.Start(TimeSpan.FromSeconds(totalSeconds));
        }

        private void ResetTimer(object? obj)
        {
            _timerService.Reset();
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}