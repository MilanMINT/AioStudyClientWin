using AioStudy.Core.Services;
using AioStudy.UI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string _text = string.Empty;
        private readonly ITimerService _timerService;
        private TimeSpan _remaining; 
        private bool _isTimerRunning;

        public bool IsTimerRunning
        {
            get { return _isTimerRunning; }
            set
            {
                _isTimerRunning = value;
                OnPropertyChanged(nameof(IsTimerRunning));
            }
        }

        public TimeSpan Remaining
        {
            get { return _remaining; }
            set
            {
                _remaining = value;
                OnPropertyChanged(nameof(Remaining));
            }
        }

        public RelayCommand StartTimerCommand { get; }
        public RelayCommand PauseTimerCommand { get; }
        public RelayCommand ResumeTimerCommand { get; }

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

        public DashboardViewModel(ITimerService timerService)
        {
            Text = "Initial Text";
            _timerService = timerService;
            _timerService.TimeChanged += (_, time) => Remaining = time;
            StartTimerCommand = new RelayCommand(StartTimer);
            PauseTimerCommand = new RelayCommand(PauseTimer);
            ResumeTimerCommand = new RelayCommand(ResumeTimer);
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
            _timerService.Start(TimeSpan.FromSeconds(15));
            IsTimerRunning = true;
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
