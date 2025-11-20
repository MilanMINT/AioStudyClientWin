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

        private QuickTimer _quickTimer1;
        private QuickTimer _quickTimer2;
        private QuickTimer _quickTimer3;

        public QuickTimer QuickTimer1
        {
            get => _quickTimer1;
            set
            {
                _quickTimer1 = value;
                OnPropertyChanged(nameof(QuickTimer1));
            }
        }
        public QuickTimer QuickTimer2
        {
            get => _quickTimer2;
            set
            {
                _quickTimer2 = value;
                OnPropertyChanged(nameof(QuickTimer2));
            }
        }
        public QuickTimer QuickTimer3
        {
            get => _quickTimer3;
            set
            {
                _quickTimer3 = value;
                OnPropertyChanged(nameof(QuickTimer3));
            }
        }

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
        public RelayCommand AddQuickTimer1Command { get; }
        public RelayCommand AddQuickTimer2Command { get; }
        public RelayCommand AddQuickTimer3Command { get; }

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

            AddQuickTimer1Command = new RelayCommand(ExecuteAddQuickTimerCommand);
            AddQuickTimer2Command = new RelayCommand(ExecuteAddQuickTimerCommand);
            AddQuickTimer3Command = new RelayCommand(ExecuteAddQuickTimerCommand);

            LoadQuickTimers();
        }

        private void ExecuteAddQuickTimerCommand(object? obj)
        {
            if (obj is string slotIdString && int.TryParse(slotIdString, out int slotId))
            {
                AddQuickTimer(slotId);
            }
            else if (obj is int slotIdInt)
            {
                AddQuickTimer(slotIdInt);
            }
        }

        private async void AddQuickTimer(int slotId)
        {
            var newQuickTimer = new QuickTimer
            {
                Slot = slotId,
                Duration = TimeSpan.FromMinutes(25)
            };

            var createdQuickTimer = await _quickTimerDbService.CreateQuickTimerAsync(newQuickTimer);

            if (createdQuickTimer != null)
            {
                switch (slotId)
                {
                    case 1:
                        QuickTimer1 = createdQuickTimer;
                        break;
                    case 2:
                        QuickTimer2 = createdQuickTimer;
                        break;
                    case 3:
                        QuickTimer3 = createdQuickTimer;
                        break;
                }
            }
        }

        private async void LoadQuickTimers()
        {
            var quickTimers = await _quickTimerDbService.GetAllQuickTimers();
            if (quickTimers != null)
            {
                _quickTimer1 = quickTimers.FirstOrDefault(qt => qt.Slot == 1) ?? null!;
                _quickTimer2 = quickTimers.FirstOrDefault(qt => qt.Slot == 2) ?? null!;
                _quickTimer3 = quickTimers.FirstOrDefault(qt => qt.Slot == 3) ?? null!;
            }
        }

        private void OnQuickTimerSelected(QuickTimer quickTimer)
        {
            QuickTimerSelected?.Invoke(this, quickTimer);
        }
    }
}
