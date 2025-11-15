using AioStudy.Core.Data.Services;
using AioStudy.Core.Util;
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
        private ObservableCollection<string> _exameStatusOptions = new();
        private List<float> _gradesFloatListToChoose = [0.7f, 1.0f, 1.3f, 1.7f, 2.0f, 2.3f, 2.7f, 3.0f, 3.3f, 3.7f, 4.0f];
        private List<string> _gradesStringListToChoose = ["0,7", "1,0", "1,3", "1,7", "2,0", "2,3", "2,7", "3,0", "3,3", "3,7", "4,0"];
        private string _selectedExamStatusOption = string.Empty;
        private string _moduleGrade;
        private string _selectedGradesStringListToChoose;

        public RelayCommand CancelAddModuleCommand { get; }
        public RelayCommand AddModuleCommand { get; }

        public string SelectedGradesStringListToChoose
        {
            get => _selectedGradesStringListToChoose;
            set
            {
                _selectedGradesStringListToChoose = value;
                OnPropertyChanged(nameof(SelectedGradesStringListToChoose));
            }
        }

        public List<string> GradesStringListToChoose
        {
            get => _gradesStringListToChoose;
            set
            {
                _gradesStringListToChoose = value;
                OnPropertyChanged(nameof(GradesStringListToChoose));
            }
        }


        public string ModuleGrade
        {
            get => _moduleGrade;
            set
            {
                _moduleGrade = value;
                OnPropertyChanged(nameof(ModuleGrade));
            }
        }

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

        public ObservableCollection<string> ExamStatusOptions
        {
            get => _exameStatusOptions;
            set
            {
                _exameStatusOptions = value;
                OnPropertyChanged(nameof(ExamStatusOptions));
            }
        }

        public string SelectedExamStatusOption
        {
            get => _selectedExamStatusOption;
            set
            {
                _selectedExamStatusOption = value;

                if (_selectedExamStatusOption == Enums.ModuleStatus.NB.ToString())
                {
                    ModuleGrade = "5,0";
                }
                else if (_selectedExamStatusOption == Enums.ModuleStatus.Open.ToString())
                {
                    ModuleGrade = string.Empty;
                }

                OnPropertyChanged(nameof(ModuleGrade));
                OnPropertyChanged(nameof(SelectedExamStatusOption));
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

            LoadExameOptions();
            SelectedExamStatusOption = ExamStatusOptions.FirstOrDefault();
        }

        private void LoadExameOptions()
        {
            foreach (string status in Enum.GetNames(typeof(Enums.ModuleStatus)))
            {
                ExamStatusOptions.Add(status);
            }
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

        private async void CancelAddModule(object? obj)
        {
            Application.Current.Windows.OfType<AddModuleView>().FirstOrDefault()?.Close();
            await ToastService.ShowInfoAsync("Info", "Cancelled adding module");
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

                float? gradeValue = null;

                if (SelectedExamStatusOption.ToString() == Enums.ModuleStatus.Open.ToString())
                {
                    gradeValue = null;
                }
                else if (SelectedExamStatusOption.ToString() == Enums.ModuleStatus.NB.ToString())
                {
                    gradeValue = 5.0f;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(SelectedGradesStringListToChoose))
                    {
                        await ToastService.ShowWarningAsync("Typo Error", "Please select a Grade");
                        return;
                    }
                    gradeValue = _gradesFloatListToChoose[GradesStringListToChoose.IndexOf(SelectedGradesStringListToChoose)];
                }

                if (!string.IsNullOrWhiteSpace(ModuleCredits) && !int.TryParse(ModuleCredits, out _))
                {
                    await ToastService.ShowWarningAsync("Typo Error", "Please enter valid Module Credits");
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
                    ModuleCredits = int.TryParse(ModuleCredits, out int credits) ? credits : null,
                    ExamStatus = SelectedExamStatusOption,
                    Grade = gradeValue
                };

                var res = await _modulesDbService.CreateModuleAsync(newModule);

                if (res is not null)
                {
                    await ToastService.ShowSuccessAsync("Success", $"Module with Name: '{ModuleName}' successfully created!");
                    _modulesViewModel.Modules.Add(res);
                    await _modulesViewModel.LoadModulesBySemesterAsync();
                    var vm = App.ServiceProvider.GetRequiredService<PomodoroViewModel>();
                    await vm.LoadModulesAsync();

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
