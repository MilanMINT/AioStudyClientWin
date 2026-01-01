using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AioStudy.UI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string[] _greetingMessages = new string[]
        {
            "Welcome back!",
            "Hello there!",
            "Good to see you!",
            "Hi! Let's get started!",
            "Ready to learn?",
            "Let's make today productive!",
            "Time to hit the books!",
            "Stay focused and keep going!"
        };


        private MainViewModel _mainViewModel;
        private readonly Dictionary<int, string> _top3TooltipPerDay = new();
        private readonly LearnSessionDbService _learnSessionDbService;
        private string _greetingsString;
        public string GreetingsString
        {
            get { return _greetingsString; }
            set
            {
                _greetingsString = value;
                OnPropertyChanged(nameof(GreetingsString));
            }
        }

        private ISeries[] _series;
        public ISeries[] Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(nameof(Series));
            }
        }

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                OnPropertyChanged(nameof(XAxes));
            }
        }

        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                OnPropertyChanged(nameof(YAxes));
            }
        }

        private readonly DispatcherTimer _rotationTimer;
        private int _currentPlotIndex = 0;
        private readonly TimeSpan _rotationInterval = TimeSpan.FromSeconds(5);
        private CancellationTokenSource? _racingCts;

        public DashboardViewModel()
        {
            _learnSessionDbService = App.ServiceProvider.GetRequiredService<LearnSessionDbService>();

            UpdateGreetingMessage();

            SetOverviewPlotData();

            _rotationTimer = new DispatcherTimer
            {
                Interval = _rotationInterval
            };
            _rotationTimer.Tick += RotationTimer_Tick;
            _rotationTimer.Start();
        }

        private void RotationTimer_Tick(object? sender, EventArgs e)
        {
            RotatePlot();
        }

        private void RotatePlot()
        {
            _racingCts?.Cancel();
            _currentPlotIndex = (_currentPlotIndex + 1) % 3;

            switch (_currentPlotIndex)
            {
                case 0:
                    SetOverviewPlotData();
                    break;
                case 1:
                    _ = SetRacingBarsPlotDataAsync();
                    break;
                case 2:
                    _ = SetWeeklyTrendPlotDataAsync();
                    break;
            }
        }

        private void SetOverviewPlotData()
        {
            var rawLearningData = new Dictionary<int, double>
            {
                { 7, 20 },
                { 8, 45 },
                { 12, 40 },
                { 17, 90 },
                { 18, 60 }
            };

            double[] learningPerHour = new double[24];

            foreach (var kv in rawLearningData)
            {
                if (kv.Key >= 0 && kv.Key <= 23)
                {
                    learningPerHour[kv.Key] = kv.Value;
                }
            }

            Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = learningPerHour,

                    Fill = new SolidColorPaint(
                        SKColor.Parse("#4FD1C7").WithAlpha(220)),
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
                    Labels = Enumerable.Range(0, 24)
                        .Select(h => h.ToString("00"))
                        .ToArray(),

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
                    SeparatorsPaint = new SolidColorPaint(
                        SKColors.White.WithAlpha(40))
                }
            };
        }

        private async Task SetRacingBarsPlotDataAsync()
        {
            try
            {
                var sessions = (await _learnSessionDbService.GetRecentSessionsAsync(-1))?.ToList() ?? new List<LearnSession>();

                var minutesPerModule = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

                foreach (var s in sessions)
                {
                    string moduleName = s.LearnedModule?.Name ?? (s.LearnedModuleId.HasValue ? $"Module {s.LearnedModuleId}" : "Without Module");
                    DateTime start = s.StartTime;
                    DateTime end = s.EndTime ?? DateTime.Now;
                    double minutes = (end - start).TotalMinutes;

                    if (!minutesPerModule.ContainsKey(moduleName))
                    {
                        minutesPerModule[moduleName] = 0;
                    }
                    minutesPerModule[moduleName] += minutes;
                }

                if (minutesPerModule.Count == 0)
                {
                    Series = new ISeries[]
                    {
                        new RowSeries<double>
                        {
                            Values = new double[] { 0 },
                            Fill = new SolidColorPaint(SKColor.Parse("#60A5FA")),
                            Stroke = null,
                            MaxBarWidth = 40,
                            YToolTipLabelFormatter = p => "Keine Daten"
                        }
                    };

                    XAxes = new[]
                    {
                        new Axis
                        {
                            Labeler = value => $"{value:0.#} h",
                            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                            Name = "Stunden"
                        }
                    };

                    YAxes = new[]
                    {
                        new Axis
                        {
                            Labels = new[] { "Keine Module" },
                            LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                            Name = "Module"
                        }
                    };

                    return;
                }

                var sorted = minutesPerModule
                    .OrderByDescending(kv => kv.Value)
                    .ToList();

                var labels = sorted.Select(kv => kv.Key).ToArray();
                var hours = sorted.Select(kv => Math.Round(kv.Value / 60.0, 2)).ToArray();

                Series = new ISeries[]
                {
                    new RowSeries<double>
                    {
                        Values = hours,
                        Fill = new SolidColorPaint(SKColor.Parse("#34D399")) { },
                        Stroke = null,
                        MaxBarWidth = 40,
                        XToolTipLabelFormatter = _ => string.Empty,
                        YToolTipLabelFormatter = p => FormatHours(p.PrimaryValue)
                    }
                };

                XAxes = new[]
                {
                    new Axis
                    {
                        Labeler = value => $"{value:0.#} h",
                        LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                        Name = "Stunden",

                        MinLimit = 0
                    }
                };

                YAxes = new[]
                {
                    new Axis
                    {
                        Labels = labels,
                        LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                        Name = "Module"
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetRacingBarsPlotDataAsync error: {ex}");
            }
        }

        private string FormatHours(double hours)
        {
            if (double.IsNaN(hours) || double.IsInfinity(hours))
                return "-";

            var totalMinutes = (int)Math.Round(hours * 60);
            int h = totalMinutes / 60;
            int m = totalMinutes % 60;
            if (h > 0 && m > 0) return $"{h}h {m}m";
            if (h > 0) return $"{h}h";
            return $"{m}m";
        }

        private async Task SetWeeklyTrendPlotDataAsync()
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime monday = today;
                while (monday.DayOfWeek != DayOfWeek.Monday)
                {
                    monday = monday.AddDays(-1);

                }
                var dayLabels = new[] { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
                double[] minutesPerDay = new double[7];

                _top3TooltipPerDay.Clear();

                for (int i = 0; i < 7; i++)
                {
                    DateTime day = monday.AddDays(i);
                    var sessions = (await _learnSessionDbService.GetSessionsByDateAsync(DateOnly.FromDateTime(day)))?.ToList() ?? new List<LearnSession>();

                    DateTime dayStart = day.Date;
                    DateTime dayEnd = dayStart.AddDays(1);

                    double totalMinutes = 0;

                    var minutesPerModule = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

                    foreach (var s in sessions)
                    {
                        DateTime start = s.StartTime;
                        DateTime end = s.EndTime ?? DateTime.Now;

                        DateTime overlapStart = start > dayStart ? start : dayStart;
                        DateTime overlapEnd = end < dayEnd ? end : dayEnd;

                        if (overlapEnd > overlapStart)
                        {
                            double overlapMinutes = (overlapEnd - overlapStart).TotalMinutes;
                            totalMinutes += overlapMinutes;

                            string moduleName = s.LearnedModule?.Name ?? (s.LearnedModuleId.HasValue ? $"Module {s.LearnedModuleId}" : "Unbekannt");
                            if (!minutesPerModule.ContainsKey(moduleName))
                            {
                                minutesPerModule[moduleName] = 0;
                            }
                            minutesPerModule[moduleName] += overlapMinutes;
                        }
                    }

                    minutesPerDay[i] = Math.Round(totalMinutes, 0);

                    var top3 = minutesPerModule
                        .OrderByDescending(kv => kv.Value)
                        .Take(3)
                        .Select(kv =>
                        {
                            var ts = TimeSpan.FromMinutes(Math.Round(kv.Value));
                            string timeStr = ts.ToString(@"h\:mm");
                            return $"{kv.Key}—{timeStr}";
                        })
                        .ToArray();

                    _top3TooltipPerDay[i] = top3.Length > 0 ? string.Join("\n", top3) : "No Data";
                }

                double[] hoursPerDay = minutesPerDay.Select(m => Math.Round(m / 60.0, 2)).ToArray();

                Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = hoursPerDay,
                        Fill = new SolidColorPaint(SKColor.Parse("#F472B6")) { },
                        Stroke = null,
                        Rx = 6,
                        Ry = 6,
                        MaxBarWidth = 40,
                        YToolTipLabelFormatter = p =>
                        {
                            int idx = (int)p.SecondaryValue;
                            string top3 = _top3TooltipPerDay.TryGetValue(idx, out var t) ? t : "No Data";
                            return $"{dayLabels[idx]} · {p.PrimaryValue:0.##}h {top3}\n";
                        }
                    }
                };

                XAxes = new[]
                {
                    new Axis
                    {
                        Labels = dayLabels,
                        LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                        SeparatorsPaint = null,
                        Name = "Weekdays (Mo–So)"
                    }
                };

                YAxes = new[]
                {
                    new Axis
                    {
                        Name = "Learning Time (Hours)",
                        MinLimit = 0,
                        LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                        SeparatorsPaint = new SolidColorPaint(SKColors.White.WithAlpha(40)),
                        Labeler = value => $"{value:0.#} h"
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"error: {ex}");
            }
        }

        private void UpdateGreetingMessage()
        {
            var random = new Random();
            int index = random.Next(_greetingMessages.Length);
            GreetingsString = _greetingMessages[index];
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public void StopRotation()
        {
            if (_rotationTimer.IsEnabled)
            {
                _rotationTimer.Stop();
                _rotationTimer.Tick -= RotationTimer_Tick;
            }
        }

        public void Dispose()
        {
            StopRotation();
        }
    }
}