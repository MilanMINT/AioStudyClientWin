using AioStudy.Core.Data.Services;
using AioStudy.Models.DailyPlannerModels;
using AioStudy.UI.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
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
        
        private readonly DailyPlanDbService _dailyPlanDbService;

        private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

        private DailyPlan _currentDailyPlan;

        public DailyPlan CurrentDailyPlan
        {
            get => _currentDailyPlan;
            set
            {
                if (_currentDailyPlan != value)
                {
                    _currentDailyPlan = value;
                    OnPropertyChanged(nameof(CurrentDailyPlan));
                }
            }
        }

        public DateOnly SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    SetCurrentDailyPlan(value);
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

            _dailyPlanDbService = App.ServiceProvider.GetRequiredService<DailyPlanDbService>();

            SelectedDate = DateOnly.FromDateTime(DateTime.Today);

            SetCurrentDailyPlan(SelectedDate);
        }

        private async void SetCurrentDailyPlan(DateOnly date)
        {
            bool __dateAlreadyExists = await _dailyPlanDbService.CheckIfPlanAlreadyExist(date);
            if (!__dateAlreadyExists)
            {
                var __newPlan = await _dailyPlanDbService.CreateDailyPlan(date);
                CurrentDailyPlan = __newPlan;
            }
        }

        private void ExecutePrevDay(object? obj)
        {
            SelectedDate = SelectedDate.AddDays(-1);
            SetCurrentDailyPlan(SelectedDate);
        }

        private void ExecuteNextDay(object? obj)
        {
            SelectedDate = SelectedDate.AddDays(1);
            SetCurrentDailyPlan(SelectedDate);
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }   
    }
}
