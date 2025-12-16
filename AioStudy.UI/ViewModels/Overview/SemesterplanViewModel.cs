using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.ViewModels;
using AioStudy.UI.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows.Threading;

namespace AioStudy.UI.ViewModels.Overview
{
    public class SemesterplanViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;
        private SemesterViewModel _semesterViewModel;

        private readonly SemesterDbService _semesterDbService;
        private readonly ModulesDbService _moduleDbService;

        private const int _parentContainerWidth = 1070;
        private const int _semesterInfoContainerWidth = 150;

        private ObservableCollection<SemesterRowViewModel> _semestersRows;
        public ObservableCollection<SemesterRowViewModel> SemesterRows
        {
            get => _semestersRows;
            set
            {
                _semestersRows = value;
                OnPropertyChanged(nameof(SemesterRows));
            }
        }



        public int ParentContainerWidth => _parentContainerWidth;
        public int SemesterInfoContainerWidth => _semesterInfoContainerWidth;

        public RelayCommand RefreshCommand => new RelayCommand(o => BuildSemesterplan());
        public RelayCommand OpenModule { get; }

        public SemesterplanViewModel(SemesterViewModel semesterViewModel, MainViewModel mainViewModel, SemesterDbService semesterDbService, ModulesDbService modulesDbService)
        {
            _semesterViewModel = semesterViewModel;
            _mainViewModel = mainViewModel;
            _semesterDbService = semesterDbService;
            _moduleDbService = modulesDbService;

            SemesterRows = new ObservableCollection<SemesterRowViewModel>();

            OpenModule = new RelayCommand(ExecuteBack);

            BuildSemesterplan();
        }

        private void ExecuteBack(object? obj)
        {
            if (obj is Module module)
            {
                var viewmodel = new ModuleOverViewViewModel(module, this);
                _mainViewModel.CurrentViewModel = viewmodel;
                _mainViewModel.CurrentViewName = $"{module.Name}´s Overview";
            }
        }

        private async void BuildSemesterplan()
        {
            SemesterRows.Clear();
            var __semesters = await _semesterDbService.GetAllSemestersAsync();
            var sortedSemesters = __semesters.OrderByDescending(s => s.StartDate).ToList();
            int totalSemesters = sortedSemesters.Count;

            foreach (Semester semester in sortedSemesters)
            {
                var __modulesBySemester = (await _semesterDbService.GetModulesForSemester(semester))?.ToList() ?? new List<Module>();
                int __totalcredits = CalculateTotalCredits(__modulesBySemester);
                int __modulesCount = __modulesBySemester.Count();

                bool isCurrentSemester = await _semesterDbService.IsCurrentSemester(semester);

                var semesterRowVM = new SemesterRowViewModel
                {
                    Semester = semester,
                    SemesterNumber = totalSemesters - sortedSemesters.IndexOf(semester),
                    Modules = new ObservableCollection<Module>(__modulesBySemester),
                    TotalCredits = __totalcredits,
                    ModulesCount = __modulesCount,
                    ParentWidth = _parentContainerWidth,
                    ModulesContainerWidth = _parentContainerWidth - _semesterInfoContainerWidth,
                    SemesterInfoContainerWidth = _semesterInfoContainerWidth,
                    IsCurrentSemester = isCurrentSemester
                };
                SemesterRows.Add(semesterRowVM);
            }
        }

        private static int CalculateTotalCredits(IEnumerable<Module> modules)
        {
            return modules.Sum(m => m.ModuleCredits ?? 0);
        }
    }
}
