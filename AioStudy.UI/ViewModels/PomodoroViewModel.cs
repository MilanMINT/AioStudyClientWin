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
        private bool _isTimerRunning;
        private bool _toggleTimerSet;

        private string _timerUpClockTime;
        private int _minutes = 25;
        private int _seconds = 0;

        private int _oldMinutes;
        private int _oldSeconds;

        private bool _isUpdatingFromTimer = false;

        public bool ToggleSetTimer
        {
            get { return _toggleTimerSet; }
            set
            {
                _toggleTimerSet = value;
                OnPropertyChanged(nameof(ToggleSetTimer));
            }
        }

        public string TimerUpClockTime
        {
            get { return _timerUpClockTime; }
            set
            {
                _timerUpClockTime = value;
                OnPropertyChanged(nameof(TimerUpClockTime));
            }
        }

        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                if (!_isUpdatingFromTimer)
                    UpdateTimerUpClockTime();
                OnPropertyChanged(nameof(Minutes));
                StartTimerCommand.RaiseCanExecuteChanged();
            }
        }

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                if (!_isUpdatingFromTimer)
                    UpdateTimerUpClockTime();
                OnPropertyChanged(nameof(Seconds));
                StartTimerCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsTimerRunning
        {
            get { return _isTimerRunning; }
            set
            {
                _isTimerRunning = value;
                OnPropertyChanged(nameof(IsTimerRunning));
                if (IsTimerRunning == false)
                {
                    UpdateTimerUpClockTime();
                }
            }
        }

        public TimeSpan Remaining
        {
            get { return _remaining; }
            set
            {
                _remaining = value;
                OnPropertyChanged(nameof(Remaining));

                if (IsTimerRunning)
                {
                    _isUpdatingFromTimer = true;
                    Minutes = (int)_remaining.TotalMinutes;
                    Seconds = _remaining.Seconds;
                    _isUpdatingFromTimer = false;
                }
                else
                {
                    UpdateTimerUpClockTime();
                }
            }
        }

        public RelayCommand StartTimerCommand { get; }
        public RelayCommand PauseTimerCommand { get; }
        public RelayCommand ResumeTimerCommand { get; }
        public RelayCommand ResetTimerCommand { get; }

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

            _timerService.TimerReset += TimerService_TimerReset;
            _timerService.TimeChanged += TimerService_TimeChanged;
            _timerService.TimerEnded += TimerService_TimerEnded;
            _timerService.RunningStateChanged += TimerService_RunningStateChanged;

            StartTimerCommand = new RelayCommand(StartTimer, CanStartTimer);
            PauseTimerCommand = new RelayCommand(PauseTimer);
            ResumeTimerCommand = new RelayCommand(ResumeTimer);
            ResetTimerCommand = new RelayCommand(ResetTimer);

            UpdateTimerUpClockTime();
            ToggleSetTimer = true;
        }

        private void TimerService_TimeChanged(object? sender, TimeSpan time)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Remaining = time;
                UpdateTimerUpClockTime();
            });
        }

        private void TimerService_TimerReset(object? sender, EventArgs e)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                IsTimerRunning = false;
                ToggleSetTimer = true;

                UpdateTimerUpClockTime();
            });
        }

        private void TimerService_TimerEnded(object? sender, EventArgs e)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                IsTimerRunning = false;
                ToggleSetTimer = true;
                Minutes = 0;
                Seconds = 0;
            });
        }

        private void TimerService_RunningStateChanged(object? sender, bool isRunning)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                IsTimerRunning = isRunning;
                CommandManager.InvalidateRequerySuggested();
            });
        }

        private bool CanStartTimer(object? arg)
        {
            if (IsTimerRunning)
            {
                return false;
            }
            else
            {
                return Minutes > 0 || Seconds > 0;
            }
        }

        private void ResumeTimer(object? obj)
        {
            System.Diagnostics.Debug.WriteLine($"[CMD Resume]");
            _timerService.Resume();
            IsTimerRunning = true;
            ToggleSetTimer = false;
        }

        private void PauseTimer(object? obj)
        {
            System.Diagnostics.Debug.WriteLine($"[CMD Pause]");
            _timerService.Pause();
            IsTimerRunning = false;
            ToggleSetTimer = true;
        }

        private void StartTimer(object? obj)
        {
            int totalSeconds = Minutes * 60 + Seconds;
            _oldMinutes = Minutes;
            _oldSeconds = Seconds;
            System.Diagnostics.Debug.WriteLine($"[CMD Start] Minutes={Minutes}, Seconds={Seconds}, Total={totalSeconds}s");
            _timerService.Start(TimeSpan.FromSeconds(totalSeconds));
            IsTimerRunning = true;
            ToggleSetTimer = false;
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        private void UpdateTimerUpClockTime()
        {
            int totalSeconds = Minutes * 60 + Seconds;
            DateTime endTime = DateTime.Now.AddSeconds(totalSeconds);
            TimerUpClockTime = endTime.ToString("HH:mm");
        }

        private void ResetTimer(object? obj)
        {
            System.Diagnostics.Debug.WriteLine($"[CMD Reset] Current Minutes={Minutes}, Seconds={Seconds}");
            _timerService.Reset();
            ToggleSetTimer = true;
            System.Diagnostics.Debug.WriteLine($"[CMD Reset] After Reset: _timerService.Remaining={_timerService.Remaining}");
            Minutes = _oldMinutes;
            Seconds = _oldSeconds;
        }
    }
}