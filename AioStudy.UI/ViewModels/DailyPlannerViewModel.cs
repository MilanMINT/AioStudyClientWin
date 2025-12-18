using AioStudy.UI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels
{
    public class DailyPlannerViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;

        private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

        public DateOnly SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        public RelayCommand NextDayCommand { get; }
        public RelayCommand PrevDayCommand { get; }

        public DailyPlannerViewModel()
        {
            NextDayCommand = new RelayCommand(ExecuteNextDay);
            PrevDayCommand = new RelayCommand(ExecutePrevDay);
        }

        private void ExecutePrevDay(object? obj)
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }

        private void ExecuteNextDay(object? obj)
        {
            SelectedDate = SelectedDate.AddDays(1);
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }   
    }
}
