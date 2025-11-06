using AioStudy.Core.Services;
using AioStudy.UI.Commands;
using System;

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
                OnPropertyChanged(nameof(Minutes));
                if (!_isUpdatingFromTimer)
                    UpdateTimerUpClockTime();
            }
        }

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                OnPropertyChanged(nameof(Seconds));
                if (!_isUpdatingFromTimer)
                    UpdateTimerUpClockTime();
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

            _timerService.TimeChanged += (_, time) =>
            {
                Remaining = time;
                UpdateTimerUpClockTime();
            };

            _timerService.TimerReset += (_, _) =>
            {
                IsTimerRunning = false;
                ToggleSetTimer = true;
                _isUpdatingFromTimer = true;
                Minutes = (int)_timerService.Remaining.TotalMinutes;
                Seconds = _timerService.Remaining.Seconds;
                _isUpdatingFromTimer = false;
                UpdateTimerUpClockTime();
            };
            _timerService.TimerEnded += OnTimerEnded;
            StartTimerCommand = new RelayCommand(StartTimer, CanStartTimer);
            PauseTimerCommand = new RelayCommand(PauseTimer);
            ResumeTimerCommand = new RelayCommand(ResumeTimer);
            ResetTimerCommand = new RelayCommand(ResetTimer);

            UpdateTimerUpClockTime();

            ToggleSetTimer = true;
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

        private void OnTimerEnded(object? sender, EventArgs e)
        {
            IsTimerRunning = false;
            ToggleSetTimer = true;
            Minutes = 0;
            Seconds = 0;
        }

        private void ResumeTimer(object? obj)
        {
            _timerService.Resume();
            IsTimerRunning = true;
            ToggleSetTimer = false;
        }

        private void PauseTimer(object? obj)
        {
            _timerService.Pause();
            IsTimerRunning = false;
            ToggleSetTimer = true;
        }

        private void StartTimer(object? obj)
        {
            int totalSeconds = Minutes * 60 + Seconds;
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
            _timerService.Reset();
        }
    }
}
