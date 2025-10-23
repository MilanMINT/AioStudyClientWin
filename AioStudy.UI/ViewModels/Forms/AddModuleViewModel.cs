using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.Views.Forms;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AioStudy.UI.ViewModels.Forms
{
    public class AddModuleViewModel : ViewModelBase
    {
        private readonly ModulesViewModel _modulesViewModel;
        private readonly ModulesDbService _modulesDbService;
        private string _moduleName = string.Empty;
        private string _moduleCredits = string.Empty;
        private DateTime? _moduleExamDate;
        private Color? _moduleColor;
        private string _moduleDescription = string.Empty;
        private Semester? _selectedSemester;
        private ObservableCollection<Semester> _semesters = new();

        public RelayCommand CancelAddModuleCommand { get; }
        public RelayCommand AddModuleCommand { get; }

        // Properties für Binding
        public string ModuleCredits
        {
            get => _moduleCredits;
            set
            {
                _moduleCredits = value;
                OnPropertyChanged(nameof(ModuleCredits));
            }
        }   

        public string ModuleName
        {
            get => _moduleName;
            set
            {
                _moduleName = value;
                OnPropertyChanged(nameof(ModuleName));
            }
        }

        public DateTime? ModuleExamDate
        {
            get => _moduleExamDate;
            set
            {
                _moduleExamDate = value;
                OnPropertyChanged(nameof(ModuleExamDate));
            }
        }

        public Color? ModuleColor
        {
            get => _moduleColor;
            set
            {
                _moduleColor = value;
                OnPropertyChanged(nameof(ModuleColor));
            }
        }

        public string ModuleDescription
        {
            get => _moduleDescription;
            set
            {
                _moduleDescription = value;
                OnPropertyChanged(nameof(ModuleDescription));
            }
        }

        public Semester? SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                _selectedSemester = value;
                OnPropertyChanged(nameof(SelectedSemester));
            }
        }

        public ObservableCollection<Semester> Semesters
        {
            get => _semesters;
            set
            {
                _semesters = value;
                OnPropertyChanged(nameof(Semesters));
            }
        }

        public AddModuleViewModel(ModulesViewModel modulesViewModel)
        {
            _modulesViewModel = modulesViewModel;
            CancelAddModuleCommand = new RelayCommand(CancelAddModule);
            AddModuleCommand = new RelayCommand(AddModule);
            _modulesDbService = App.ServiceProvider.GetRequiredService<ModulesDbService>();

            // Semester laden
            _ = LoadSemestersAsync();
        }



        private async Task LoadSemestersAsync()
        {
            try
            {
                var semesterService = App.ServiceProvider.GetRequiredService<SemesterDbService>();
                var allSemesters = await semesterService.GetAllSemestersAsync();
                
                Semesters.Clear();
                foreach (var semester in allSemesters)
                {
                    Semesters.Add(semester);
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync("Error", $"Could not load Semesters: {ex.Message}");
            }
        }

        private void CancelAddModule(object? obj)
        {
            Application.Current.Windows.OfType<AddModuleView>().FirstOrDefault()?.Close();
        }

        private async void AddModule(object? obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ModuleName))
                {
                    await ToastService.ShowWarningAsync("Typo Error", "Please enter a Modulename");
                    return;
                }

                string? colorString = null;
                if (ModuleColor.HasValue)
                {
                    colorString = ModuleColor.Value.ToString();
                }

                var newModule = new Module(ModuleName)
                {
                    ExamDate = ModuleExamDate,
                    Color = colorString,
                    SemesterId = SelectedSemester?.Id,
                    ModuleCredits = int.TryParse(ModuleCredits, out int credits) ? credits : null
                };
                
                var res = await _modulesDbService.CreateModuleAsync(newModule);

                if (res is not null)
                {
                    await ToastService.ShowSuccessAsync("Success", $"Module with Name: '{ModuleName}' successfully created!");
                    _modulesViewModel.Modules.Add(res);
                    await _modulesViewModel.LoadModulesBySemesterAsync();
                    
                    Application.Current.Windows.OfType<AddModuleView>().FirstOrDefault()?.Close();
                }
                else
                {
                    await ToastService.ShowErrorAsync("Error", $"Module with Name '{ModuleName}' could not be created!");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync("Error", $"Couldnt create Module: {ex.Message}");
            }
        }
    }
}
