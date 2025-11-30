using AioStudy.UI.Commands;
using System;
using System.ComponentModel;

namespace AioStudy.UI.ViewModels.Components
{
    public class TimerOverlayViewModel : ViewModelBase
    {
        private readonly PomodoroViewModel _pomodoroViewModel;
        private bool _isVisible;

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

        public RelayCommand CloseCommand { get; }
        public RelayCommand ControlTimerCommand { get; }
        public RelayCommand ResetTimerCommand { get; }

        public TimerOverlayViewModel(PomodoroViewModel pomodoroViewModel)
        {
            _pomodoroViewModel = pomodoroViewModel;

            CloseCommand = new RelayCommand(_ => IsVisible = false);
            ControlTimerCommand = new RelayCommand(_ => _pomodoroViewModel.ControlTimerCommand?.Execute(null));
            ResetTimerCommand = new RelayCommand(_ => _pomodoroViewModel.ResetTimerCommand?.Execute(null));

            _pomodoroViewModel.PropertyChanged += OnPomodoroPropertyChanged;
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
                    }
                    break;
                case nameof(PomodoroViewModel.IsPaused):
                    OnPropertyChanged(nameof(IsPaused));
                    break;
            }
        }
    }
}