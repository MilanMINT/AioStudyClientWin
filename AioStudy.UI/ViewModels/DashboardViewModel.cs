using AioStudy.Core.Services;
using AioStudy.UI.Commands;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;

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

        public ISeries[] Series { get; }
        public Axis[] XAxes { get; }
        public Axis[] YAxes { get; }

        public DashboardViewModel()
        {
            UpdateGreetingMessage();

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
                    learningPerHour[kv.Key] = kv.Value;
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
    }
}
