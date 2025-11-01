using AioStudy.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels
{
    public class GradesViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;
        private readonly ITimerService _timerService;
        private TimeSpan _remaining;

        public TimeSpan Remaining
        {
            get { return _remaining; }
            set
            {
                _remaining = value;
                OnPropertyChanged(nameof(Remaining));
            }
        }

        public GradesViewModel(ITimerService timerService)
        {
            _timerService = timerService;
            _timerService.TimeChanged += (_, time) => Remaining = time;
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
