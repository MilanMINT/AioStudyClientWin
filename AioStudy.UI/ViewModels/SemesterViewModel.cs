using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels.Forms;
using AioStudy.UI.ViewModels.Overview;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace AioStudy.UI.ViewModels
{
    public class SemesterViewModel : ViewModelBase
    {
        private readonly SemesterDbService _semesterDbService;
        private readonly ModulesDbService _modulesDbService;
        private MainViewModel _mainViewModel;
        private ModulesViewModel _modulesViewModel;
        private readonly SemaphoreSlim _loadSemaphore = new SemaphoreSlim(1, 1);
        private List<Semester> _allSemesters = new();

        private string _searchQuery = string.Empty;
        private ObservableCollection<Semester> _semesters;
        private bool _isLoading;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                FilterSemester();
            }
        }

        public ObservableCollection<Semester> Semesters
        {
            get { return _semesters; }
            set
            {
                _semesters = value;
                OnPropertyChanged(nameof(Semesters));
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public RelayCommand LoadSemestersCommand { get; }
        public RelayCommand AddSemesterCommand { get; }
        public RelayCommand DeleteSemesterCommand { get; }
        public RelayCommand OpenSpecificModulesViewCMD { get; }
        public RelayCommand OpenSemesterOverviewCommand { get; }
        public RelayCommand OpenSemesterplanCommand { get; }

        public SemesterViewModel(SemesterDbService semesterDbService, ModulesViewModel modulesViewModel, ModulesDbService modulesDbService)
        {
            _semesterDbService = semesterDbService;
            _modulesViewModel = modulesViewModel;
            _modulesDbService = modulesDbService;
            Semesters = new ObservableCollection<Semester>();

            LoadSemestersCommand = new RelayCommand(async _ => await LoadSemestersAsync());
            AddSemesterCommand = new RelayCommand(async _ => await CreateSemesterAsync());
            DeleteSemesterCommand = new RelayCommand(async param => await DeleteSemesterWithConfirmation(param));
            OpenSemesterOverviewCommand = new RelayCommand(_ => OpenSemesterOverview());
            OpenSemesterplanCommand = new RelayCommand(ExecuteOpenSemesterplanCommand);

            _ = LoadSemestersAsync();
        }

        private void ExecuteOpenSemesterplanCommand(object? obj)
        {
            var viewmodel = new SemesterplanViewModel(this, _mainViewModel, _semesterDbService, _modulesDbService);
            _mainViewModel.CurrentViewModel = viewmodel;
            _mainViewModel.CurrentViewName = $"Semester Plan";
        }

        private void FilterSemester()
        {
            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                Semesters.Clear();
                foreach (var semester in _allSemesters)
                {
                    Semesters.Add(semester);
                }
            }
            else
            {
                var query = _searchQuery.ToLower();
                var filtered = _allSemesters.Where(m => m.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();

                Semesters.Clear();
                foreach (var semester in filtered)
                {
                    Semesters.Add(semester);
                }
            }
        }

        private void OpenSemesterOverview()
        {
            MessageBox.Show("Open!");
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        private async Task LoadSemestersAsync()
        {
            if (!await _loadSemaphore.WaitAsync(0))
            {
                System.Diagnostics.Debug.WriteLine("LoadSemestersAsync bereits aktiv, überspringe...");
                return;
            }

            try
            {
                IsLoading = true;
                
                var semesters = await _semesterDbService.GetAllSemestersAsync();

                _allSemesters.Clear();
                Semesters.Clear();
                
                foreach (var semester in semesters)
                {
                    _allSemesters.Add(semester);
                    Semesters.Add(semester);
                    
                    var modulesCount = await _semesterDbService.GetModulesCountForSemester(semester);
                    semester.ModulesCount = modulesCount;
                }

                if (!string.IsNullOrWhiteSpace(_searchQuery))
                {
                    FilterSemester();
                }

                OnPropertyChanged(nameof(Semesters));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler beim Laden der Semester: {ex.Message}");
                MessageBox.Show($"Fehler beim Laden der Semester: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                _loadSemaphore.Release();
            }
        }

        private async Task CreateSemesterAsync()
        {
            var addWindow = new AddSemesterView();
            var viewModel = App.ServiceProvider.GetRequiredService<AddSemesterViewModel>();
            addWindow.DataContext = viewModel;
            addWindow.Owner = Application.Current.MainWindow;
            addWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addWindow.ShowDialog();
        }

        private async Task DeleteSemesterWithConfirmation(object parameter)
        {
            if (parameter is Semester semester)
            {
                bool confirmed = ConfirmModalService.ShowDeleteConfirmation(semester.Name);

                if (confirmed)
                {
                    await DeleteSemesterAsync(semester);
                    await ToastService.ShowSuccessAsync("Semester Deleted!", $"The semester '{semester.Name}' has been successfully deleted.");
                    await _mainViewModel._pomodoroViewModel.LoadRecentSessionsAsync();
                }
            }
        }

        private async Task DeleteSemesterAsync(object parameter)
        {
            if (parameter is Semester semester)
            {
                try
                {
                    bool success = await _semesterDbService.DeleteSemester(semester.Id);
                    if (success)
                    {
                        Semesters.Remove(semester);
                        _allSemesters.Remove(semester);
                        await _modulesViewModel.LoadModulesBySemesterAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Fehler beim Löschen des Semesters: {ex.Message}");
                }
            }
        }
        public void RefreshSemesters()
        {
            _ = LoadSemestersAsync();
        }   
    }
}
