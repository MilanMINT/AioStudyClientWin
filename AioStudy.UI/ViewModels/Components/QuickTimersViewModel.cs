using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Components
{
    public class QuickTimersViewModel : ViewModelBase
    {
        private ObservableCollection<QuickTimer> _quickTimers;
        private QuickTimer _selectedQuickTimer;
        private readonly QuickTimerDbService _quickTimerDbService;

        public ObservableCollection<QuickTimer> QuickTimers
        {
            get => _quickTimers;
            set
            {
                _quickTimers = value;
                OnPropertyChanged(nameof(QuickTimers));
            }
        }
        public QuickTimer SelectedQuickTimer
        {
            get => _selectedQuickTimer;
            set
            {
                _selectedQuickTimer = value;
                OnPropertyChanged(nameof(SelectedQuickTimer));
            }
        }

        public event EventHandler<QuickTimer> QuickTimerSelected;
        public RelayCommand SelectQuickTimerCommand { get; }

        public QuickTimersViewModel(QuickTimerDbService quickTimerDbService)
        {
            _quickTimers = new ObservableCollection<QuickTimer>();
            _quickTimerDbService = quickTimerDbService;

            SelectQuickTimerCommand = new RelayCommand(param =>
            {
                if (param is QuickTimer quickTimer)
                {
                    OnQuickTimerSelected(quickTimer);
                }
            });
            LoadQuickTimers();
        }

        private async void LoadQuickTimers()
        {
            var quickTimers = await _quickTimerDbService.GetAllQuickTimers();
            if (quickTimers != null)
            {
                QuickTimers = new ObservableCollection<QuickTimer>(quickTimers);
            }
        }

        private void OnQuickTimerSelected(QuickTimer quickTimer)
        {
            QuickTimerSelected?.Invoke(this, quickTimer);
        }
    }
}
