using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Core.Util;
using AioStudy.UI.Commands;
using System;
using System.ComponentModel;

namespace AioStudy.UI.ViewModels.Components
{
    public class TimerOverlayViewModel : ViewModelBase
    {
        private readonly PomodoroViewModel _pomodoroViewModel;
        private readonly ITimerService _timerService;
        private bool _isVisible;
        private string _gradientPopoutColor1 = "#3A3D45"; // Blau 1
        private string _gradientPopoutColor2 = "#3A3D45"; // Blau 2
        private string _gradientPopoutColor3 = "#a5bacc"; // Blau 3

        private string _timerStatusString = "Timer stopped";

        public string TimerStatusString
        {
            get => _timerStatusString;
            set
            {
                _timerStatusString = value;
                OnPropertyChanged(nameof(TimerStatusString));
            }
        }

        public string GradientPopoutColor1
        {
            get => _gradientPopoutColor1;
            set
            {
                _gradientPopoutColor1 = value;
                OnPropertyChanged(nameof(GradientPopoutColor1));
            }
        }

        public string GradientPopoutColor2
        {
            get => _gradientPopoutColor2;
            set
            {
                _gradientPopoutColor2 = value;
                OnPropertyChanged(nameof(GradientPopoutColor2));
            }
        }

        public string GradientPopoutColor3
        {
            get => _gradientPopoutColor3;
            set
            {
                _gradientPopoutColor3 = value;
                OnPropertyChanged(nameof(GradientPopoutColor3));
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public int Minutes => _pomodoroViewModel.Minutes;
        public int Seconds => _pomodoroViewModel.Seconds;
        public bool IsRunning => _pomodoroViewModel.IsRunning;
        public bool IsPaused => _pomodoroViewModel.IsPaused;
        public bool IsBreakActive => _pomodoroViewModel.IsBreakActive;

        public RelayCommand CloseCommand { get; }
        public RelayCommand ControlTimerCommand { get; }
        public RelayCommand ResetTimerCommand { get; }

        public TimerOverlayViewModel(PomodoroViewModel pomodoroViewModel, ITimerService timerService)
        {

            _pomodoroViewModel = pomodoroViewModel;
            _timerService = timerService;
            _pomodoroViewModel.SetTimerOverlayViewModel(this);
            CloseCommand = new RelayCommand(_ => IsVisible = false);
            ControlTimerCommand = new RelayCommand(_ => _pomodoroViewModel.ControlTimerCommand?.Execute(null), _ => !IsBreakActive);
            ResetTimerCommand = new RelayCommand(_ => _pomodoroViewModel.ResetTimerCommand?.Execute(null));

            _pomodoroViewModel.PropertyChanged += OnPomodoroPropertyChanged;
            
        }

        private void SetGradientColors(string color1, string color2, string color3)
        {
            GradientPopoutColor1 = color1;
            GradientPopoutColor2 = color2;
            GradientPopoutColor3 = color3;
        }

        public void ApplyGradientScheme(GradientColorSchemes.GradientColors colors)
        {
            GradientColorSchemes.ApplyColors(colors, SetGradientColors);
        }

        private void OnPomodoroPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PomodoroViewModel.Minutes):
                    OnPropertyChanged(nameof(Minutes));
                    break;
                case nameof(PomodoroViewModel.Seconds):
                    OnPropertyChanged(nameof(Seconds));
                    break;
                case nameof(PomodoroViewModel.IsRunning):
                    OnPropertyChanged(nameof(IsRunning));
                    if (!IsRunning)
                    {
                        IsVisible = false;
                        TimerStatusString = "Timer stopped";
                    }
                    else
                    {
                        TimerStatusString = "Timer running";
                    }
                        break;
                case nameof(PomodoroViewModel.IsPaused):
                    ApplyGradientScheme(GradientColorSchemes.Timer.Red);
                    OnPropertyChanged(nameof(IsPaused));
                    if (IsPaused)
                    {
                        TimerStatusString = "Timer paused";
                    }
                    else
                    {
                        TimerStatusString = "Timer running";
                    }
                    break;
                case nameof(PomodoroViewModel.IsShortBreakActive):
                    if (!string.IsNullOrEmpty(_pomodoroViewModel.IsShortBreakActive))
                    {
                        ApplyGradientScheme(GradientColorSchemes.Timer.Yellow);
                        TimerStatusString = "Short break active";
                    }
                    break;
                case nameof(PomodoroViewModel.IsMidBreakActive):
                    if (!string.IsNullOrEmpty(_pomodoroViewModel.IsMidBreakActive))
                    {
                        ApplyGradientScheme(GradientColorSchemes.Timer.Orange);
                        TimerStatusString = "Mid break active";
                    }
                        break;
                case nameof(PomodoroViewModel.IsLongBreakActive):
                    if (!string.IsNullOrEmpty(_pomodoroViewModel.IsLongBreakActive))
                    {
                        ApplyGradientScheme(GradientColorSchemes.Timer.Green);
                        TimerStatusString = "Long break active";
                    }
                    break;
                case nameof(PomodoroViewModel.IsBreakActive):
                    if (!_pomodoroViewModel.IsBreakActive)
                    {
                        TimerStatusString = "Timer running";
                    }
                    break;
            }
        }
    }
}