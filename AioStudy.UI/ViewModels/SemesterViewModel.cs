using AioStudy.Core.Data.Services;
using AioStudy.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AioStudy.UI.Commands;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.ViewModels.Forms;
using System.Windows.Media.Animation;
using System.Windows;

namespace AioStudy.UI.ViewModels
{
    public class SemesterViewModel : ViewModelBase
    {
        private readonly SemesterDbService _semesterDbService;
        private MainViewModel _mainViewModel;
        private ModulesViewModel _modulesViewModel;

        private ObservableCollection<Semester> _semesters;
        private bool _isLoading;

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

        public SemesterViewModel(SemesterDbService semesterDbService, ModulesViewModel modulesViewModel)
        {
            _semesterDbService = semesterDbService;
            _modulesViewModel = modulesViewModel;
            Semesters = new ObservableCollection<Semester>();

            LoadSemestersCommand = new RelayCommand(async _ => await LoadSemestersAsync());
            AddSemesterCommand = new RelayCommand(async _ => await AddSampleSemesterAsync());
            DeleteSemesterCommand = new RelayCommand(async param => await DeleteSemesterAsync(param));
            OpenSemesterOverviewCommand = new RelayCommand(_ => OpenSemesterOverview());
            //OpenSpecificModulesViewCMD = new RelayCommand(OpenModulesBySemester);

            _ = LoadSemestersAsync();
        }

        private void OpenSemesterOverview()
        {
            MessageBox.Show("Open!");
        }

        //private void OpenModulesBySemester(object? obj)
        //{
        //    if (obj is Semester semester)
        //    {
        //        try
        //        {
        //            var modulesViewModel = App.ServiceProvider.GetRequiredService<ModulesViewModel>();
        //            modulesViewModel.SetSelectedSemester(semester);
        //            _mainViewModel.CurrentViewModel = modulesViewModel;
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }
        //}

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        private async Task LoadSemestersAsync()
        {
            IsLoading = true;
            try
            {
                var semesters = await _semesterDbService.GetAllSemestersAsync();
                
                Semesters.Clear();
                var fillTasks = new List<Task>();
                foreach (var semester in semesters)
                {
                    Semesters.Add(semester);

                    fillTasks.Add(Task.Run(async () =>
                    {
                        var modulesCount = await _semesterDbService.GetModulesCountForSemester(semester);
                        semester.ModulesCount = modulesCount;
                    }));
                }

                await Task.WhenAll(fillTasks);
                OnPropertyChanged(nameof(Semesters));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler beim Laden der Semester: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddSampleSemesterAsync()
        {
            //var addWindow = new AddSemesterView();
            //var viewModel = App.ServiceProvider.GetRequiredService<AddSemesterViewModel>();
            //addWindow.DataContext = viewModel;
            //addWindow.Owner = System.Windows.Application.Current.MainWindow;
            //addWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            //addWindow.ShowDialog();
            try
            {
                var newSemester = await _semesterDbService.CreateSemesterAsync(
                    $"SemesterWiSe {DateTime.Now:yyyy-MM}",
                    DateTime.Now.ToUniversalTime(),  // Convert to UTC
                    DateTime.Now.AddMonths(16).ToUniversalTime()  // Convert to UTC
                );

                Semesters.Add(newSemester);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler beim Erstellen des Semesters: {ex.Message}");
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
                        await _modulesViewModel.LoadModulesBySemesterAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Fehler beim Löschen des Semesters: {ex.Message}");
                }
            }
        }
    }
}
