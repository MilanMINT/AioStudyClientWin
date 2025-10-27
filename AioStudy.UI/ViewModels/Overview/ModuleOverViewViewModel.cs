using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AioStudy.UI.ViewModels.Overview
{
    public class ModuleOverViewViewModel : ViewModelBase
    {
        private Module _module;
        private ModulesDbService _modulesDbService;
        private ModulesViewModel _modulesViewModel;
        private MainViewModel _mainViewModel;
        private string _dayTillExam;
        private string _learnTimeGoal;

        public string LearnTimeGoal
        {
            get
            {
                if (_module.ModuleCredits == null || _module.ModuleCredits == 0)
                {
                    return "No Credits Assigned";
                }
                int totalMinutesGoal = _module.ModuleCredits.Value * 30 * 60 - 900;
                var ts = TimeSpan.FromMinutes(totalMinutesGoal);
                int hours = (int)ts.TotalHours;
                int minutes = ts.Minutes;
                return $"{hours}h {minutes}m";

            }
            set
            {
                _learnTimeGoal = value;
                OnPropertyChanged(nameof(LearnTimeGoal));
            }
        }

        public string LearnedMinutes
        {
            get
            {
                int totalMinutes = _module?.LearnedMinutes ?? 0;
                var ts = TimeSpan.FromMinutes(totalMinutes);
                int hours = (int)ts.TotalHours;
                int minutes = ts.Minutes;
                return $"{hours}h {minutes}m";
            }
            set
            {
                if (_module != null)
                {
                    if (TimeSpan.TryParse(value, out var ts))
                    {
                        _module.LearnedMinutes = (int)ts.TotalMinutes;
                    }
                    OnPropertyChanged(nameof(LearnedMinutes));
                }
            }
        }

        public int TotalMinutesGoal
        {
            get
            {
                if (_module == null || _module.ModuleCredits == null || _module.ModuleCredits == 0)
                    return 0;
                return _module.ModuleCredits.Value * 30 * 60 - 900;
            }
        }

        public string DayTillExam
        {
            get
            {
                if (_module.ExamDate == null)
                {
                    return "No Date Set";
                }
                DateTime today = DateTime.Today;
                DateTime examDate = _module.ExamDate.Value;

                TimeSpan diff = examDate - today;
                int daysDiff = (int)diff.TotalDays;

                return daysDiff switch
                {
                    > 0 when daysDiff == 1 => "1 day remaining",
                    > 0 => $"{daysDiff} days remaining",
                    0 => "Exam is today!",
                    -1 => "Exam was 1 day ago",
                    < 0 => $"Exam was {Math.Abs(daysDiff)} days ago"
                };
            }
            set
            {
                _dayTillExam = value;
                OnPropertyChanged(nameof(DayTillExam));
            }
        }

        public Module Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
                OnPropertyChanged(nameof(ModuleName));

            }
        }

        public string ModuleName
        {
            get { return _module?.Name ?? "Unknown Module"; }
            set
            {
                if (_module != null)
                {
                    _module.Name = value;
                    OnPropertyChanged(nameof(ModuleName));
                }
            }
        }

        public RelayCommand BackCommand { get; }
        public RelayCommand DeleteModuleCommand { get; }
        public RelayCommand EditModuleCommand { get; }
        public RelayCommand AddMin { get; }

        public ModuleOverViewViewModel(Module module, ModulesViewModel modulesViewModel, MainViewModel mainViewModel, RelayCommand deleteModuleCommand)
        {
            Module = module;
            _modulesDbService = App.ServiceProvider.GetRequiredService<ModulesDbService>();
            _modulesViewModel = modulesViewModel;
            _mainViewModel = mainViewModel;
            
            // Wrapper Command für Delete mit Navigation
            DeleteModuleCommand = new RelayCommand(async parameter => await DeleteModuleWithNavigation(parameter));
            AddMin = new RelayCommand(async parameter => await UpMin(parameter));

            // Commands
            BackCommand = new RelayCommand(ExecuteBackCommand);
        }

        private async Task UpMin(object parameter)
        {
            int minutesToAdd = 0;
            if (parameter is int i)
            {
                minutesToAdd = i;
            }
            else if (parameter is string s && int.TryParse(s, out var parsed))
            {
                minutesToAdd = parsed;
            }
            else if (parameter != null && int.TryParse(parameter.ToString(), out parsed))
            {
                minutesToAdd = parsed;
            }
            else
            {
                return;
            }

            if (_module == null)
                return;

            _module.LearnedMinutes += minutesToAdd;

            // Persistieren
            var ok = await _modulesDbService.UpdateModuleAsync(_module);

            OnPropertyChanged(nameof(Module));
            OnPropertyChanged(nameof(TotalMinutesGoal));
            OnPropertyChanged(nameof(LearnTimeGoal));
            OnPropertyChanged(nameof(LearnedMinutes));
        }

        private async Task DeleteModuleWithNavigation(object? parameter)
        {
            if (parameter is Module module)
            {
                bool confirmed = ConfirmModalService.ShowDeleteConfirmation(module.Name);

                if (confirmed)
                {
                    try
                    {
                        bool success = await _modulesDbService.DeleteModule(module.Id);
                        if (success)
                        {
                            // Remove from ModulesViewModel collection
                            _modulesViewModel.Modules.Remove(module);
                            
                            // Show success message
                            await ToastService.ShowSuccessAsync("Module Deleted!", $"The module '{module.Name}' has been successfully deleted.");
                            
                            // Navigate back to modules view
                            _mainViewModel.CurrentViewModel = _modulesViewModel;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Fehler beim Löschen des Moduls: {ex.Message}");
                    }
                }
            }
        }

        private void ExecuteBackCommand(object? obj)
        {
            _mainViewModel.CurrentViewModel = _modulesViewModel;
        }
    }
}
