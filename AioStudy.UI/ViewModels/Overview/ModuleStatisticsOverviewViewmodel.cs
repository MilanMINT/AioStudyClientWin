using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Overview
{
    public class ModuleStatisticsOverviewViewmodel : ViewModelBase
    {
        private Module _module;
        private ModuleOverViewViewModel _moduleOverViewViewModel;
        private MainViewModel _mainViewModel;
        private readonly LearnSessionDbService _learnSessionDbService;

        private string _moduleName;
        private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

        private ISeries[] _series = Array.Empty<ISeries>();
        private Axis[] _xAxes = Array.Empty<Axis>();
        private Axis[] _yAxes = Array.Empty<Axis>();

        public ISeries[] Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(nameof(Series));
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

        public DateOnly SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                    LoadStatistics();
                }
            }
        }

        public string ModuleName
        {
            get { return _moduleName; }
            set
            {
                _moduleName = value;
                OnPropertyChanged(nameof(ModuleName));
            }
        }

        public RelayCommand BackCommand { get; }
        public RelayCommand NextDayCommand { get; }
        public RelayCommand PrevDayCommand { get; }

        public ModuleStatisticsOverviewViewmodel(Module module, ModuleOverViewViewModel moduleOverViewViewModel, MainViewModel mainViewModel)
        {
            _module = module;
            _moduleOverViewViewModel = moduleOverViewViewModel;
            _mainViewModel = mainViewModel;
            _learnSessionDbService = App.ServiceProvider.GetRequiredService<LearnSessionDbService>();

            ModuleName = module.Name;

            BackCommand = new RelayCommand(ExecuteBackCommand);
            NextDayCommand = new RelayCommand(ExecuteNextDay);
            PrevDayCommand = new RelayCommand(ExecutePrevDay);

            SelectedDate = DateOnly.FromDateTime(DateTime.Today);
            LoadStatistics();
        }

        public async void LoadStatistics()
        {
            try
            {
                var sessions = (await _learnSessionDbService.GetSessionsByDateAsync(SelectedDate, _module))?.ToList() ?? new List<LearnSession>();
                System.Diagnostics.Debug.WriteLine($"LoadStatistics: sessions.Count = {sessions.Count}");

                int[] timePerHours = ExtractTimePerHoursFromModuleSession(sessions, SelectedDate);
                System.Diagnostics.Debug.WriteLine($"Sum minutes: {timePerHours.Sum()}");

                Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = timePerHours.Select(i => (double)i).ToArray(),
                        Fill = new SolidColorPaint(SKColor.Parse("#4FD1C7").WithAlpha(220)),
                        Stroke = null,
                        Rx = 6,
                        Ry = 6,
                        MaxBarWidth = 22,

                        XToolTipLabelFormatter = _ => string.Empty,

                        YToolTipLabelFormatter = point =>
                        {
                            int hour = (int)point.SecondaryValue;
                            return $"{hour:00}–{hour + 1:00} Uhr · {point.PrimaryValue:0} min";
                        }
                    }
                };

                XAxes = new[]
                {
                    new Axis
                    {
                        Labels = Enumerable.Range(0, 24).Select(h => h.ToString("00")).ToArray(),
                        MinStep = 1,
                        ForceStepToMin = true,
                        SeparatorsPaint = null,
                        LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                        Name = "Uhrzeit"
                    }
                };

                YAxes = new[]
                {
                    new Axis
                    {
                        Name = "Lernzeit (Min)",
                        MinLimit = 0,
                        LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                        SeparatorsPaint = new SolidColorPaint(SKColors.White.WithAlpha(40))
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadStatistics error: {ex}");
            }
        }

        private int[] ExtractTimePerHoursFromModuleSession(IEnumerable<LearnSession> learnSessions, DateOnly date)
        {
            var timePerHours = new int[24];

            DateTime dayStart = date.ToDateTime(new TimeOnly(0, 0));
            DateTime dayEnd = dayStart.AddDays(1);

            foreach (var session in learnSessions)
            {
                System.Diagnostics.Debug.WriteLine($"TIME: {session.StartTime} - {session.EndTime}: {session.EndTime - session.StartTime}");
                DateTime start = session.StartTime;
                DateTime end = session.EndTime ?? DateTime.Now;

                DateTime overlapStart = start > dayStart ? start : dayStart;
                DateTime overlapEnd = end < dayEnd ? end : dayEnd;

                if (overlapEnd <= overlapStart)
                {
                    continue;
                }

                for (int hour = 0; hour < 24; hour++)
                {
                    DateTime hourStart = dayStart.AddHours(hour);
                    DateTime hourEnd = hourStart.AddHours(1);

                    DateTime effectiveStart = overlapStart > hourStart ? overlapStart : hourStart;
                    DateTime effectiveEnd = overlapEnd < hourEnd ? overlapEnd : hourEnd;

                    if (effectiveEnd > effectiveStart)
                    {
                        timePerHours[hour] += (int)(effectiveEnd - effectiveStart).TotalMinutes;
                    }
                }
            }

            return timePerHours;
        }


        private void ExecutePrevDay(object? obj)
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }

        private void ExecuteNextDay(object? obj)
        {
            SelectedDate = SelectedDate.AddDays(1);
        }

        private void ExecuteBackCommand(object? obj)
        {
            _mainViewModel.CurrentViewModel = _moduleOverViewViewModel;
            _mainViewModel.CurrentViewName = $"{_module.Name}´s Overview";
        }
    }
}
