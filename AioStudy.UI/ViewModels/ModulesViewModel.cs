using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels.Forms;
using AioStudy.UI.ViewModels.Overview;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using OpenTK.Graphics.OpenGL;
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
        private readonly ModulesDbService _modulesDbService;

        private ObservableCollection<Module> _modules;

        public RelayCommand DeleteModuleCommand { get; }
        public RelayCommand CreateModuleCommand { get; }
        public RelayCommand OpenModuleOverviewCommand { get; }
        public RelayCommand RefreshModulesCommand { get; }

        private string _semesterName;

        private Semester _selectedSemester;

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

        public ModulesViewModel(ModulesDbService modulesDbService)
        {
            _modulesDbService = modulesDbService;
            Modules = new ObservableCollection<Module>();
            DeleteModuleCommand = new RelayCommand(async parameter => await DeleteModuleWithConfirmation(parameter)); 
            CreateModuleCommand = new RelayCommand(async _ => await CreateModule());
            OpenModuleOverviewCommand = new RelayCommand(async parameter => await OpenModuleOverview(parameter));
            RefreshModulesCommand = new RelayCommand(async _ => await LoadModulesBySemesterAsync());
            _ = LoadModulesBySemesterAsync();
        }

        private async Task OpenModuleOverview(object? parameter)
        {
            if (parameter is Module module)
            {
                var editWindow = new AddModuleView();
                var viewmodel = new ModuleOverViewViewModel(module, this, _mainViewModel, DeleteModuleCommand);
                _mainViewModel.CurrentViewModel = viewmodel;
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

                Modules.Clear();
                foreach (var module in modules)
                {
                    if (module.SemesterId.HasValue)
                    {
                        module.Semester = allSemesters.FirstOrDefault(s => s.Id == module.SemesterId.Value);
                    }
                    Modules.Add(module);
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
