using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels.Forms;
using AioStudy.UI.Views.Forms;
using Microsoft.Extensions.DependencyInjection;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            DeleteModuleCommand = new RelayCommand(async param => await DeleteModuleAsync(param));
            CreateModuleCommand = new RelayCommand(async _ => await CreateModule());
            _ = LoadModulesBySemesterAsync();
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

            //await LoadModulesBySemesterAsync();

            //try
            //{
            //    var newModule = new Module
            //    {
            //        Name = "Neues Modul",
            //        SemesterId = null,
            //        SemesterCredits = 4,
            //        Color = "#FF5733"

            //    };
            //    await _modulesDbService.CreateModuleAsync(newModule);
            //    Modules.Add(newModule);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Fehler beim Erstellen des Moduls: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            //    throw;
            //}
        }
    }
}
