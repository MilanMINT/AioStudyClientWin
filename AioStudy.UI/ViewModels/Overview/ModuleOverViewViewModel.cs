using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Core.Util;
using AioStudy.Core.Util.Modules;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Overview
{
    public class ModuleOverViewViewModel : ViewModelBase
    {
        private Module _module;
        private readonly ITimerService _timerService;
        private readonly LearnSessionDbService _learnSessionDbService;
        private readonly SemesterDbService _semesterDbService;
        private readonly ModulesDbService _modulesDbService;
        private ObservableCollection<LearnSession> _recentSessions = new ObservableCollection<LearnSession>();
        private ModulesViewModel _modulesViewModel;
        private MainViewModel _mainViewModel;
        private ViewModelBase _previousViewModel;
        private string _dayTillExam;
        private string _learnTimeGoal;
        private string _averageSessionTime;
        private string _sessionCount;
        private string _backToString;
        private bool _isEditMode = false;

        //Edit Props
        private ObservableCollection<Semester> _semesters = new ObservableCollection<Semester>();
        private string? _newCreditValue = string.Empty;
        private DateTime? _newExameDate;
        private string? _newModuleName = string.Empty;
        private Semester? _newSemester;
        private Enums.ModuleStatus? _newModuleStatus;
        private float? _newModuleGrade;
        private Color? _newModuleColor;
        private Semester _newSelectedSemester;

        private readonly List<float> _gradesFloatListToChoose = new()
        {
            0.7f, 1.0f, 1.3f, 1.7f, 2.0f, 2.3f, 2.7f, 3.0f, 3.3f, 3.7f, 4.0f
        };
        private readonly List<string> _gradesStringListToChoose = new()
        {
            "0,7","1,0","1,3","1,7","2,0","2,3","2,7","3,0","3,3","3,7","4,0"
        };
        private string? _selectedGradesStringListToChoose;

        public Semester NewSelectedSemester
        {
            get { return _newSelectedSemester; }
            set
            {
                _newSelectedSemester = value;
                OnPropertyChanged(nameof(NewSelectedSemester));
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

        public string? NewCreditValue
        {
            get { return _newCreditValue; }
            set
            {
                _newCreditValue = value;
                OnPropertyChanged(nameof(NewCreditValue));
            }
        }

        public DateTime? NewExameDate
        {
            get { return _newExameDate; }
            set
            {
                _newExameDate = value;
                OnPropertyChanged(nameof(NewExameDate));
            }
        }

        public string? NewModuleName
        {
            get { return _newModuleName; }
            set
            {
                _newModuleName = value;
                OnPropertyChanged(nameof(NewModuleName));
            }
        }

        public Semester? NewSemester
        {
            get { return _newSemester; }
            set
            {
                _newSemester = value;
                OnPropertyChanged(nameof(NewSemester));
            }
        }

        public Enums.ModuleStatus? NewModuleStatus
        {
            get { return _newModuleStatus; }
            set
            {
                _newModuleStatus = value;
                // adjust grade depending on status
                if (_newModuleStatus == Enums.ModuleStatus.NB)
                {
                    NewModuleGrade = 5.0f;
                    SelectedGradesStringListToChoose = null;
                }
                else if (_newModuleStatus == Enums.ModuleStatus.Open)
                {
                    NewModuleGrade = null;
                    SelectedGradesStringListToChoose = null;
                }
                else if (_newModuleStatus == Enums.ModuleStatus.BE)
                {
                    // keep existing grade (already handled in ResetEditProperties)
                }
                OnPropertyChanged(nameof(NewModuleStatus));
                OnPropertyChanged(nameof(ShowGradeEdit));
            }
        }

        public float? NewModuleGrade
        {
            get { return _newModuleGrade; }
            set
            {
                _newModuleGrade = value;
                OnPropertyChanged(nameof(NewModuleGrade));
            }
        }

        public Color? NewModuleColor
        {
            get { return _newModuleColor; }
            set
            {
                _newModuleColor = value;
                OnPropertyChanged(nameof(NewModuleColor));
            }
        }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(ShowGradeEdit));
            }
        }

        // Expose grade strings for ComboBox
        public List<string> GradesStringListToChoose => _gradesStringListToChoose;

        public string? SelectedGradesStringListToChoose
        {
            get => _selectedGradesStringListToChoose;
            set
            {
                _selectedGradesStringListToChoose = value;
                // map selected string to float grade
                if (!string.IsNullOrEmpty(_selectedGradesStringListToChoose))
                {
                    var idx = _gradesStringListToChoose.IndexOf(_selectedGradesStringListToChoose);
                    if (idx >= 0 && idx < _gradesFloatListToChoose.Count)
                    {
                        NewModuleGrade = _gradesFloatListToChoose[idx];
                    }
                }
                else
                {
                    // no selection -> do not change grade when BE? set null
                    NewModuleGrade = null;
                }
                OnPropertyChanged(nameof(SelectedGradesStringListToChoose));
            }
        }

        // used for visibility in XAML: show grade selector only when editing AND status == BE
        public bool ShowGradeEdit => IsEditMode && NewModuleStatus == Enums.ModuleStatus.BE;

        public ObservableCollection<LearnSession> RecentSessions
        {
            get { return _recentSessions; }
            set
            {
                _recentSessions = value;
                OnPropertyChanged(nameof(RecentSessions));
            }
        }

        public string BackToString
        {
            get { return _backToString; }
            set
            {
                _backToString = value;
                OnPropertyChanged(nameof(BackToString));
            }
        }

        public string AverageSessionTime
        {
            get { return _averageSessionTime; }
            set
            {
                _averageSessionTime = value;
                OnPropertyChanged(nameof(AverageSessionTime));
            }
        }

        public string SessionCount
        {
            get { return _sessionCount; }
            set
            {
                _sessionCount = value;
                OnPropertyChanged(nameof(SessionCount));
            }
        }

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
        public RelayCommand OpenModuleStatisticsCommand { get; }
        public RelayCommand ToggleEditCancelCommand { get; }
        public RelayCommand SaveModuleCommand { get; }

        public ModuleOverViewViewModel(Module module, ViewModelBase previousViewModel)
        {
            Module = module;
            _modulesDbService = App.ServiceProvider.GetRequiredService<ModulesDbService>();
            _modulesViewModel = App.ServiceProvider.GetRequiredService<ModulesViewModel>();
            _mainViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
            _learnSessionDbService = App.ServiceProvider.GetRequiredService<LearnSessionDbService>();
            _semesterDbService = App.ServiceProvider.GetRequiredService<SemesterDbService>();
            _previousViewModel = previousViewModel;

            DeleteModuleCommand = new RelayCommand(async parameter => await DeleteModuleWithNavigation(parameter));
            OpenModuleStatisticsCommand = new RelayCommand(ExecuteOpenModuleStatisticsCommand);
            ToggleEditCancelCommand = new RelayCommand(ExecuteToggleSaveCancelCommand);
            SaveModuleCommand = new RelayCommand(ExecuteSaveModuleAfterEditing);

            Semesters = new ObservableCollection<Semester>(_semesterDbService.GetAllSemestersAsync().Result);

            // Commands
            BackCommand = new RelayCommand(ExecuteBackCommand);

            _timerService = App.ServiceProvider.GetRequiredService<ITimerService>();
            _timerService.TimerEnded += OnTimerEndet;

            SetBackToString();

            RefreshAll();
        }

        private void ResetEditProperties()
        {
            NewModuleName = _module.Name;
            NewCreditValue = _module.ModuleCredits?.ToString() ?? string.Empty;
            NewExameDate = _module.ExamDate;

            // ensure NewSelectedSemester references the instance from Semesters collection
            if (_module.Semester != null)
            {
                var match = Semesters.FirstOrDefault(s => s.Id == _module.Semester.Id);
                NewSelectedSemester = match ?? _module.Semester;
            }
            else
            {
                NewSelectedSemester = null;
            }

            // keep NewSemester for other uses (if you still need it)
            NewSemester = _module.Semester;

            if (Enum.TryParse<Enums.ModuleStatus>(_module.ExamStatus, out var status))
            {
                NewModuleStatus = status;
            }
            else
            {
                NewModuleStatus = null;
            }

            NewModuleGrade = _module.Grade;

            try
            {
                if (NewModuleGrade.HasValue)
                {
                    var rounded = MathF.Round(NewModuleGrade.Value, 1);
                    var idx = _gradesFloatListToChoose.IndexOf(rounded);
                    if (idx >= 0 && idx < _gradesStringListToChoose.Count)
                    {
                        SelectedGradesStringListToChoose = _gradesStringListToChoose[idx];
                    }
                    else
                    {
                        SelectedGradesStringListToChoose = null;
                    }
                }
                else
                {
                    SelectedGradesStringListToChoose = null;
                }
            }
            catch
            {
                SelectedGradesStringListToChoose = null;
            }

            if (!string.IsNullOrEmpty(_module.Color))
            {
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(_module.Color);
                    NewModuleColor = color;
                }
                catch
                {
                    NewModuleColor = null;
                }
            }
            else
            {
                NewModuleColor = null;
            }

            // notify dependent bindings
            OnPropertyChanged(nameof(SelectedGradesStringListToChoose));
            OnPropertyChanged(nameof(ShowGradeEdit));
            OnPropertyChanged(nameof(NewSelectedSemester));
        }

        private void ExecuteToggleSaveCancelCommand(object? obj)
        {
            if (IsEditMode)
            {
                IsEditMode = false;
                ResetEditProperties();
            }
            else
            {
                IsEditMode = true;
                ResetEditProperties();
            }
        }

        private void ExecuteSaveModuleAfterEditing(object? obj)
        {
            if (_module != null)
            {
                _module.Name = NewModuleName ?? _module.Name;
                if (int.TryParse(NewCreditValue, out int credits))
                {
                    _module.ModuleCredits = credits;
                }
                else
                {
                    _module.ModuleCredits = null;
                }

                _module.ExamDate = NewExameDate;

                // IMPORTANT: set both navigation and FK so the DB update persists the semester
                _module.Semester = NewSelectedSemester;
                _module.SemesterId = NewSelectedSemester?.Id;

                _module.ExamStatus = NewModuleStatus?.ToString() ?? _module.ExamStatus;
                _module.Grade = NewModuleGrade;
                if (NewModuleColor.HasValue)
                {
                    _module.Color = NewModuleColor.Value.ToString();
                }
                else
                {
                    _module.Color = null;
                }

                _ = _modulesDbService.UpdateModuleAsync(_module);

                IsEditMode = false;
                ResetEditProperties();
                OnPropertyChanged(nameof(Module));
                OnPropertyChanged(nameof(ModuleName));
            }

        }

        private void SetBackToString()
        {
            switch (_previousViewModel)
            {
                case ModulesViewModel:
                    BackToString = "Modules";
                    break;
                case DashboardViewModel:
                    BackToString = "Dashboard";
                    break;
                case GradesViewModel:
                    BackToString = "Grades";
                    break;
                case SemesterViewModel:
                    BackToString = "Semester";
                    break;
                case ModuleStatisticsOverviewViewmodel:
                    BackToString = "Statistics Overview";
                    break;
                case SemesterplanViewModel:
                    BackToString = "Semesterplan";
                    break;
                // fallback
                default:
                    BackToString = "Modules";
                    break;
            }
        }

        private void ExecuteOpenModuleStatisticsCommand(object? obj)
        {
            var moduleStatisticsOverviewViewmodel = new ModuleStatisticsOverviewViewmodel(_module, this, _mainViewModel);
            _mainViewModel.CurrentViewModel = moduleStatisticsOverviewViewmodel;
            _mainViewModel.CurrentViewName = $"{_module.Name}´s Statistics";
        }

        private void OnTimerEndet(object? sender, EventArgs e)
        {
            RefreshAll();
        }

        public async Task LoadRecentSessionsAsync()
        {
            RecentSessions.Clear();
            var sessions = await _learnSessionDbService.GetRecentSessionsAsync(3, _module);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var session in sessions)
                {
                    RecentSessions.Add(session);
                }
            });
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
                            _modulesViewModel.Modules.Remove(module);

                            await ToastService.ShowSuccessAsync("Module Deleted!", $"The module '{module.Name}' has been successfully deleted.");

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

        private void LoadSessionsCountAsync(IEnumerable<LearnSession> sessions)
        {
            try
            {
                SessionCount = sessions.Count().ToString();
            }
            catch (Exception)
            {
                SessionCount = "0";
            }
        }

        private void LoadAverageSessionTime(IEnumerable<LearnSession> sessions)
        {
            try
            {
                var avgTime = ModuleHelper.Maths.CalculateAverageSessionTime(sessions);
                int hours = (int)avgTime.TotalHours;
                int minutes = avgTime.Minutes;
                AverageSessionTime = $"{hours}h {minutes}m";
            }
            catch (Exception)
            {
                AverageSessionTime = "0h 0m";
            }
        }

        public void RefreshAll()
        {
            _ = UpdateSessionStats();
            _ = LoadRecentSessionsAsync();
        }

        private async Task UpdateSessionStats()
        {
            var sessions = await _learnSessionDbService.GetRecentSessionsAsync(-1, _module);

            LoadSessionsCountAsync(sessions);
            LoadAverageSessionTime(sessions);
        }

        private void ExecuteBackCommand(object? obj)
        {
            _mainViewModel.CurrentViewModel = _previousViewModel;

            switch (_previousViewModel)
            {
                case ModulesViewModel:
                    _mainViewModel.CurrentViewName = "Modules";
                    break;

                case DashboardViewModel:
                    _mainViewModel.CurrentViewName = "Dashboard";
                    break;

                case GradesViewModel:
                    _mainViewModel.CurrentViewName = "Grades";
                    break;

                case SemesterViewModel:
                    _mainViewModel.CurrentViewName = "Semester";
                    break;
                case SemesterplanViewModel:
                    _mainViewModel.CurrentViewName = "Semesterplan";
                    break;
                case ModuleStatisticsOverviewViewmodel:
                    _mainViewModel.CurrentViewName = $"{_module.Name}´s Statistics";
                    break;

                // fallback
                default:
                    _mainViewModel.CurrentViewName = "Modules";
                    break;
            }
        }
    }
}