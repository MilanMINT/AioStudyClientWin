using AioStudy.Core.Data.Services;
using AioStudy.Core.Util.Grades;
using AioStudy.Models;
using AioStudy.UI.Commands;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows;

namespace AioStudy.UI.ViewModels
{
    public class GradesViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;
        private readonly SemesterDbService _semesterDbService;

        private ObservableCollection<Semester> _semesters = [];
        private ObservableCollection<Module> _gradedModules = [];
        private Semester? _selectedSemester;

        private double _averageGrade;
        private int _totalCredits;
        private int _totalModules;
        private double _bestGrade;
        private double _worstGrade;
        private bool _includeFailedBool = false;

        private static readonly double[] GradeSteps = [0.7, 1.0, 1.3, 1.7, 2.0, 2.3, 2.7, 3.0, 3.3, 3.7, 4.0, 5.0];

        public Semester? SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                _selectedSemester = value;
                OnPropertyChanged(nameof(SelectedSemester));
                DisplayHeaderData();
            }
        }
        public ObservableCollection<Module> GradedModules
        {
            get => _gradedModules;
            set
            {
                _gradedModules = value;
                OnPropertyChanged(nameof(GradedModules));
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

        public int TotalCredits
        {
            get => _totalCredits;
            set
            {
                _totalCredits = value;
                OnPropertyChanged(nameof(TotalCredits));
            }
        }

        public int TotalModules
        {
            get => _totalModules;
            set
            {
                _totalModules = value;
                OnPropertyChanged(nameof(TotalModules));
            }
        }

        public double BestGrade
        {
            get => _bestGrade;
            set
            {
                _bestGrade = value;
                OnPropertyChanged(nameof(BestGrade));
            }
        }

        public double WorstGrade
        {
            get => _worstGrade;
            set
            {
                _worstGrade = value;
                OnPropertyChanged(nameof(WorstGrade));
            }
        }

        public bool IncludeFailedBool
        {
            get => _includeFailedBool;
            set
            {
                _includeFailedBool = value;
                DisplayHeaderData();
                OnPropertyChanged(nameof(IncludeFailedBool));
            }
        }

        public double AverageGrade
        {
            get => _averageGrade;
            set
            {
                _averageGrade = value;
                OnPropertyChanged(nameof(AverageGrade));
            }
        }

        private ISeries[] _gradesSeries = [];
        private ISeries[] _gradeTrendSeries = [];
        private ISeries[] _gradeDistributionSeries = [];
        private Axis[] _xAxes = [];
        private Axis[] _yAxes = [];

        public ISeries[] GradesSeries
        {
            get => _gradesSeries;
            set
            {
                _gradesSeries = value;
                OnPropertyChanged(nameof(GradesSeries));
            }
        }

        public ISeries[] GradeTrendSeries
        {
            get => _gradeTrendSeries;
            set
            {
                _gradeTrendSeries = value;
                OnPropertyChanged(nameof(GradeTrendSeries));
            }
        }

        public ISeries[] GradeDistributionSeries
        {
            get => _gradeDistributionSeries;
            set
            {
                _gradeDistributionSeries = value;
                OnPropertyChanged(nameof(GradeDistributionSeries));
            }
        }

        public Axis[] XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                OnPropertyChanged(nameof(XAxes));
            }
        }

        public Axis[] YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                OnPropertyChanged(nameof(YAxes));
            }
        }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand ClearSemesterCommand { get; }
        public RelayCommand ExportGradeListCommand { get; }

        public GradesViewModel(SemesterDbService semesterDbService)
        {
            _semesterDbService = semesterDbService;

            RefreshCommand = new RelayCommand(RefreshData);
            ClearSemesterCommand = new RelayCommand(_ => SelectedSemester = null);
            ExportGradeListCommand = new RelayCommand(ExportGradeList);

            InitializeAxes();
            _ = LoadSemesterAsync();
            DisplayHeaderData();
        }

        private void ExportGradeList(object? obj)
        {
            MessageBox.Show("Not implemented!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RefreshData(object? obj)
        {
            _ = LoadSemesterAsync();
            DisplayHeaderData();
        }

        public async Task LoadSemesterAsync()
        {
            try
            {
                var semesters = await _semesterDbService.GetAllSemestersAsync();
                Semesters.Clear();
                foreach (var semester in semesters)
                {
                    Semesters.Add(semester);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading semesters: {ex.Message}");
            }
        }

        public async void DisplayHeaderData()
        {
            try
            {
                var modules = await _semesterDbService.GetModulesForSemester(SelectedSemester);
                var moduleList = modules.ToList();

                DisplayGPA(moduleList);
                DisplayTotalCredits(moduleList);
                DisplayModuleStats(moduleList);
                UpdateGradeDistributionChart(moduleList);
                UpdatePieChart(moduleList);
                UpdateGradedModulesList(moduleList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private void UpdateGradedModulesList(List<Module> modules)
        {
            var gradedModules = modules
                .Where(m => (m.Grade.HasValue && m.Grade > 0) || m.IsKeyCompetence)
                .OrderBy(m => m.IsKeyCompetence)
                .ThenBy(m => m.Grade ?? 999)
                .ToList();

            if (!IncludeFailedBool)
            {
                gradedModules = gradedModules
                    .Where(m => m.IsKeyCompetence || (m.Grade.HasValue && m.Grade <= 4.0))
                    .ToList();
            }

            GradedModules.Clear();
            foreach (var module in gradedModules)
            {
                GradedModules.Add(module);
            }
        }

        private void DisplayModuleStats(List<Module> modules)
        {
            var gradedModules = modules.Where(m => m.Grade.HasValue && m.Grade > 0);

            if (!IncludeFailedBool)
            {
                gradedModules = gradedModules.Where(m => m.Grade <= 4.0);
            }

            var gradedModulesList = gradedModules.ToList();

            TotalModules = gradedModulesList.Count;

            if (gradedModulesList.Any())
            {
                BestGrade = gradedModulesList.Min(m => m.Grade ?? 5.0);
                WorstGrade = gradedModulesList.Max(m => m.Grade ?? 1.0);
            }
            else
            {
                BestGrade = 0;
                WorstGrade = 0;
            }
        }

        public void DisplayTotalCredits(IEnumerable<Module> modules)
        {
            try
            {
                TotalCredits = GradeHelper.Math.CalculateTotalCredits(modules, IncludeFailedBool);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }

        private void DisplayGPA(IEnumerable<Module> modules)
        {
            try
            {
                AverageGrade = GradeHelper.Math.CalculateGPA(modules, IncludeFailedBool);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }

        private void InitializeAxes()
        {
            XAxes =
            [
                new Axis
                {
                    Labels = GradeSteps.Select(g => g.ToString("F1").Replace(".", ",")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#B0B3BA")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#3A3D45")),
                    LabelsRotation = 0,
                    MinStep = 1,
                    ForceStepToMin = true
                }
            ];

            YAxes =
            [
                new Axis
                {
                    MinLimit = 0,
                    MinStep = 1,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#B0B3BA")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#3A3D45"))
                }
            ];

            GradesSeries =
            [
                new ColumnSeries<int>
                {
                    Name = "Anzahl",
                    Values = new int[GradeSteps.Length],
                    Fill = new SolidColorPaint(SKColor.Parse("#608BC1")),
                    Stroke = null,
                    MaxBarWidth = 30,
                    Padding = 3
                }
            ];

            GradeDistributionSeries = [];

            GradeTrendSeries =
            [
                new LineSeries<double>
                {
                    Name = "Durchschnitt",
                    Values = [],
                    Stroke = new SolidColorPaint(SKColor.Parse("#4FD1C7")) { StrokeThickness = 3 },
                    Fill = null,
                    GeometryFill = new SolidColorPaint(SKColor.Parse("#4FD1C7")),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#4FD1C7")) { StrokeThickness = 2 },
                    GeometrySize = 8
                }
            ];
        }

        private void UpdateGradeDistributionChart(List<Module> modules)
        {
            var gradeCounts = new int[GradeSteps.Length];

            var gradedModules = modules.Where(m => m.Grade.HasValue && m.Grade > 0);

            if (!IncludeFailedBool)
            {
                gradedModules = gradedModules.Where(m => m.Grade <= 4.0);
            }

            foreach (var module in gradedModules)
            {
                var grade = module.Grade!.Value;
                var index = FindClosestGradeIndex(grade);
                if (index >= 0 && index < gradeCounts.Length)
                {
                    gradeCounts[index]++;
                }
            }

            GradesSeries =
            [
                new ColumnSeries<int>
                {
                    Name = "Anzahl",
                    Values = gradeCounts,
                    Fill = new SolidColorPaint(SKColor.Parse("#608BC1")),
                    Stroke = null,
                    MaxBarWidth = 30,
                    Padding = 3
                }
            ];
        }

        private void UpdatePieChart(List<Module> modules)
        {
            var gradedModules = modules.Where(m => m.Grade.HasValue && m.Grade > 0);

            if (!IncludeFailedBool)
            {
                gradedModules = gradedModules.Where(m => m.Grade <= 4.0);
            }

            var moduleList = gradedModules.ToList();

            var gradeCounts = new int[GradeSteps.Length];

            foreach (var module in moduleList)
            {
                var grade = module.Grade!.Value;
                var index = FindClosestGradeIndex(grade);
                if (index >= 0 && index < gradeCounts.Length)
                {
                    gradeCounts[index]++;
                }
            }

            var series = new List<ISeries>();

            for (int i = 0; i < GradeSteps.Length; i++)
            {
                if (gradeCounts[i] > 0)
                {
                    var gradeValue = GradeSteps[i];
                    var gradeLabel = gradeValue.ToString("F1").Replace(".", ",");
                    var color = GetGradeColor(gradeValue);

                    series.Add(new PieSeries<int>
                    {
                        Name = gradeLabel,
                        Values = [gradeCounts[i]],
                        Fill = new SolidColorPaint(SKColor.Parse(color)),
                        DataLabelsSize = 12,
                        DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#FFFFFF")),
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                    });
                }
            }

            GradeDistributionSeries = series.ToArray();
        }

        private int FindClosestGradeIndex(double grade)
        {
            for (int i = 0; i < GradeSteps.Length; i++)
            {
                if (System.Math.Abs(grade - GradeSteps[i]) < 0.15)
                {
                    return i;
                }
            }

            double minDiff = double.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < GradeSteps.Length; i++)
            {
                var diff = System.Math.Abs(grade - GradeSteps[i]);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        private static string GetGradeColor(double grade)
        {
            return grade switch
            {
                0.7 => "#22C55E",  // Dunkelgrün
                1.0 => "#4ADE80",  // Grün
                1.3 => "#86EFAC",  // Hellgrün
                1.7 => "#5EEAD4",  // Türkis-Grün
                2.0 => "#2DD4BF",  // Türkis
                2.3 => "#67E8F9",  // Hell-Türkis
                2.7 => "#FACC15",  // Gelb
                3.0 => "#FCD34D",  // Hellgelb
                3.3 => "#FB923C",  // Orange
                3.7 => "#F97316",  // Dunkelorange
                4.0 => "#EF4444",  // Rot
                5.0 => "#DC2626",  // Dunkelrot
                _ => "#6B7280"     // Grau (Fallback)
            };
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}