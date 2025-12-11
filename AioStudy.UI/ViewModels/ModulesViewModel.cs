using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels.Forms;
using AioStudy.UI.ViewModels.Overview;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.Views.Overviews;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AioStudy.UI.ViewModels
{
    public class ModulesViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;
        private GradesViewModel _gradesViewModel;
        private readonly LearnSessionDbService _learnSessionDbService;
        private readonly ModulesDbService _modulesDbService;
        private ObservableCollection<Module> _modules;
        private readonly ITimerService _timerService;
        private List<Module> _allModules = new();
        private string _searchQuery = string.Empty;

        public RelayCommand DeleteModuleCommand { get; }
        public RelayCommand CreateModuleCommand { get; }
        public RelayCommand OpenModuleOverviewCommand { get; }
        public RelayCommand RefreshModulesCommand { get; }
        public RelayCommand OpenAllModuleStatisticsCommand { get; }

        private string _semesterName;

        private Semester _selectedSemester;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                FilterModules();
            }
        }

        public ObservableCollection<Module> Modules
        {
            get { return _modules; }
            set
            {
                _modules = value;
                OnPropertyChanged(nameof(Modules));
            }
        }

        public string SemesterName
        {
            get { return _semesterName; }
            set
            {
                _semesterName = value;
                OnPropertyChanged(nameof(SemesterName));
            }
        }

        public ModulesViewModel(ModulesDbService modulesDbService, GradesViewModel gradesViewModel, LearnSessionDbService learnSessionDbService, ITimerService timerService)
        {
            _modulesDbService = modulesDbService;
            _gradesViewModel = gradesViewModel;
            _learnSessionDbService = learnSessionDbService;
            _timerService = timerService;
            Modules = new ObservableCollection<Module>();
            DeleteModuleCommand = new RelayCommand(async parameter => await DeleteModuleWithConfirmation(parameter)); 
            CreateModuleCommand = new RelayCommand(async _ => await CreateModule());
            OpenModuleOverviewCommand = new RelayCommand(async parameter => await OpenModuleOverview(parameter));
            RefreshModulesCommand = new RelayCommand(RefreshModules);
            OpenAllModuleStatisticsCommand = new RelayCommand(OpenAllModuleStatisticsPage);

            _ = LoadModulesBySemesterAsync();

            _timerService.MinuteElapsed += OnMinuteElapsed;
        }

        private void OpenAllModuleStatisticsPage(object? obj)
        {
            //var statsWindow = new AllModulesStatisticsView();
            var viewmodel = new AllModulesStatisticsViewViewModel();
            _mainViewModel.CurrentViewModel = viewmodel;
            _mainViewModel.CurrentViewName = $"Module Statistics";
        }

        private async void OnMinuteElapsed(object? sender, EventArgs e)
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadModulesBySemesterAsync();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ModulesVM] Error refreshing modules: {ex.Message}");
            }
        }

        private void RefreshModules(object? obj)
        {
            _ = LoadModulesBySemesterAsync();
        }

        private void FilterModules()
        {
            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                Modules.Clear();
                foreach (var module in _allModules)
                {
                    Modules.Add(module);
                }
            }
            else
            {
                var query = _searchQuery.ToLower();
                var filtered = _allModules.Where(m =>
                    m.Name.ToLower().Contains(query) ||
                    (m.Semester?.Name?.ToLower().Contains(query) ?? false)
                ).ToList();

                Modules.Clear();
                foreach (var module in filtered)
                {
                    Modules.Add(module);
                }
            }
        }

        private async Task OpenModuleOverview(object? parameter)
        {
            if (parameter is Module module)
            {
                var viewmodel = new ModuleOverViewViewModel(module, this, _mainViewModel, _learnSessionDbService, _timerService);
                _mainViewModel.CurrentViewModel = viewmodel;
                _mainViewModel.CurrentViewName = $"{module.Name}´s Overview";
            }
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public async Task LoadModulesBySemesterAsync()
        {
            try
            {
                var modules = await _modulesDbService.GetAllModulesAsync();
                var semesterService = App.ServiceProvider.GetRequiredService<SemesterDbService>();
                var allSemesters = await semesterService.GetAllSemestersAsync();

                _allModules.Clear();
                Modules.Clear();
                foreach (var module in modules)
                {
                    if (module.SemesterId.HasValue)
                    {
                        module.Semester = allSemesters.FirstOrDefault(s => s.Id == module.SemesterId.Value);
                    }
                    _allModules.Add(module);
                    Modules.Add(module);
                }

                if (!string.IsNullOrWhiteSpace(_searchQuery))
                {
                    FilterModules();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task DeleteModuleWithConfirmation(object parameter)
        {
            if (parameter is Module module)
            {
                bool confirmed = ConfirmModalService.ShowDeleteConfirmation(module.Name);

                if (confirmed)
                {
                    await DeleteModuleAsync(module);
                    await ToastService.ShowSuccessAsync("Module Deleted!", $"The module '{module.Name}' has been successfully deleted.");
                    await _mainViewModel._pomodoroViewModel.LoadRecentSessionsAsync();
                    _gradesViewModel.DisplayHeaderData();
                }
            }
        }

        private async Task DeleteModuleAsync(object parameter)
        {
            if (parameter is Module module)
            {
                try
                {
                    bool success = await _modulesDbService.DeleteModule(module.Id);
                    if (success)
                    {
                        Modules.Remove(module);
                        _allModules.Remove(module);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Fehler beim Löschen des Moduls: {ex.Message}");
                }
            }
        }

        private async Task CreateModule()
        {
            var addWindow = new AddModuleView();
            var viewModel = App.ServiceProvider.GetRequiredService<AddModuleViewModel>();
            addWindow.DataContext = viewModel;
            addWindow.Owner = System.Windows.Application.Current.MainWindow;
            addWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            addWindow.ShowDialog();
        }
    }
}
