using AioStudy.Core.Manager.Settings;
using AioStudy.Core.Services;
using AioStudy.Core.Util;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class TimerService : ITimerService
    {
        private readonly SettingsManager _settingsManager = SettingsManager.Instance;

        private DateTime _endTime;
        private DateTime _breakEndTime;
        private TimeSpan _remaining;
        private TimeSpan _breakRemaining;
        private TimeSpan _breakDuration;
        private TimeSpan _remainingBeforeBreak;

        public TimeSpan BreakDuration => _breakDuration;
        public TimeSpan BreakRemaining => _breakRemaining;
        public TimeSpan Remaining => _remaining;
        public DateTime BreakEndTime => _breakEndTime;
        public DateTime EndTime => _endTime;

        private bool _isRunning;
        public bool IsRunning => _isRunning;

        private bool _isPaused = false;
        private bool _isBreak = false;
        public bool IsPaused => _isPaused;
        public bool IsBreak => _isBreak;

        private readonly object _sync = new();

        public event EventHandler<TimeSpan> TimeChanged;
        public event EventHandler TimerEnded;
        public event EventHandler<bool> RunningStateChanged;
        public event EventHandler<bool> PausedStateChanged;
        public event EventHandler BreakEnded;
        public event EventHandler<Enums.TimerBreakType> BreakStateChanged;
        public event EventHandler Last10Seconds;

        private DateTime _startTime;
        private TimeSpan _totalDuration;
        private int _activeMinutesLogged = 0;

        private LearnSession? _currentSession;
        private Module? _currentModule;
        private DailyModuleStats? _currentDailyModuleStats;

        private readonly UserDbService _userDbService;
        private readonly ModulesDbService _modulesDbService;
        private readonly LearnSessionDbService _learnSessionDbService;
        private readonly DailyModuleStatsDbService _dailyModuleStatsDbService;

        private CancellationTokenSource? _cts;


        public TimerService(UserDbService userDbService, ModulesDbService modulesDbService, LearnSessionDbService learnSessionDbService, DailyModuleStatsDbService dailyModuleStatsDbService)
        {
            _userDbService = userDbService;
            _modulesDbService = modulesDbService;
            _learnSessionDbService = learnSessionDbService;
            _dailyModuleStatsDbService = dailyModuleStatsDbService;
        }


        #region Timer Control Methods
        // ------------------------------------------------------------------------------------
        public void Start(TimeSpan duration, Module? module = null)
        {
            _remaining = duration;
            _totalDuration = duration;
            _startTime = DateTime.UtcNow;
            _endTime = DateTime.UtcNow.Add(duration);
            _cts = new CancellationTokenSource();
            _isRunning = true;
            _isPaused = false;
            _activeMinutesLogged = 0;
            _isBreak = false;
            RunningStateChanged?.Invoke(this, true);
            PausedStateChanged?.Invoke(this, false);
            _currentSession = null;
            _currentModule = module;
            _currentSession = _learnSessionDbService.CreateLearnSessionAsync(module).GetAwaiter().GetResult() ?? throw new Exception("Failed to create learn session");

            if (module != null)
            {
                _currentDailyModuleStats = _dailyModuleStatsDbService.CreateDailyModuleStatIfNotExist(module).GetAwaiter().GetResult();
            }

            if (_currentDailyModuleStats != null)
            {
                _ = _dailyModuleStatsDbService.IncrementSessionCountAsync(_currentDailyModuleStats);
            }

            _ = RunLoopAsync(_cts.Token);
        }

        public void Pause()
        {
            _isPaused = true;
            PausedStateChanged?.Invoke(this, true);
        }

        public async void Reset()
        {
            if (_currentSession != null)
            {
                await _learnSessionDbService.CancelSessionAsync(_currentSession);
            }

            _isRunning = false;
            _isBreak = false;
            Stop();
        }

        public void Resume()
        {
            _endTime = DateTime.UtcNow.Add(_remaining);
            _isPaused = false;
            PausedStateChanged?.Invoke(this, false);
        }

        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
                _isBreak = false;
                TimeChanged?.Invoke(this, TimeSpan.Zero);
                TimerEnded?.Invoke(this, EventArgs.Empty);
                RunningStateChanged?.Invoke(this, false);
                _currentDailyModuleStats = null;
                _isRunning = false;
                TimeChanged?.Invoke(this, _totalDuration);
            }
        }
        // ------------------------------------------------------------------------------------
        #endregion



        #region Break Methods
        // ------------------------------------------------------------------------------------
        public void StartBreak(Enums.TimerBreakType breakType)
        {
            BreakStateChanged?.Invoke(this, breakType);
            switch (breakType)
            {
                case Enums.TimerBreakType.Short:
                    _breakDuration = TimeSpan.FromMinutes(_settingsManager.Settings.BreakDurationsInMinutes[0]);
                    ExecuteBreak();
                    break;
                case Enums.TimerBreakType.Mid:
                    _breakDuration = TimeSpan.FromMinutes(_settingsManager.Settings.BreakDurationsInMinutes[1]);
                    ExecuteBreak();
                    break;
                case Enums.TimerBreakType.Long:
                    _breakDuration = TimeSpan.FromMinutes(_settingsManager.Settings.BreakDurationsInMinutes[2]);
                    ExecuteBreak();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(breakType), breakType, null);
            }
        }

        private void ExecuteBreak()
        {
            lock (_sync)
            {
                _remainingBeforeBreak = _remaining;
                _isBreak = true;
                _breakEndTime = DateTime.UtcNow.Add(_breakDuration);
                _breakRemaining = _breakDuration;
            }
        }

        public void EndBreak()
        {
            lock (_sync)
            {
                _breakRemaining = TimeSpan.Zero;
                _isBreak = false;
                _endTime = DateTime.UtcNow.Add(_remainingBeforeBreak);
                _remaining = _remainingBeforeBreak;
                BreakEnded?.Invoke(this, EventArgs.Empty);
                TimeChanged?.Invoke(this, TimeSpan.Zero);
            }
        }
        // ------------------------------------------------------------------------------------
        #endregion



        #region Async Loop
        // ------------------------------------------------------------------------------------
        private async Task RunLoopAsync(CancellationToken ct)
        {
            var lastSecond = -1;

            while (!ct.IsCancellationRequested)
            {
                bool paused;
                bool isBreak;
                lock (_sync)
                {
                    paused = _isPaused;
                    isBreak = _isBreak;
                }

                if (isBreak)
                {
                    var breakRemaining = _breakEndTime - DateTime.UtcNow;

                    if (breakRemaining.TotalSeconds <= 0)
                    {
                        lock (_sync)
                        {
                            _breakRemaining = TimeSpan.Zero;
                            _isBreak = false;
                            _endTime = DateTime.UtcNow.Add(_remainingBeforeBreak);
                            _remaining = _remainingBeforeBreak;
                            BreakEnded?.Invoke(this, EventArgs.Empty);
                        }

                        TimeChanged?.Invoke(this, TimeSpan.Zero);
                        await Task.Delay(100, ct);
                        continue;
                    }

                    var breakRoundedRemaining = TimeSpan.FromSeconds(Math.Ceiling(breakRemaining.TotalSeconds));
                    lock (_sync)
                    {
                        _breakRemaining = breakRoundedRemaining;
                    }

                    TimeChanged?.Invoke(this, breakRoundedRemaining);
                    await Task.Delay(100, ct);
                    continue;
                }

                if (paused)
                {
                    await Task.Delay(100, ct);
                    continue;
                }

                var remaining = _endTime - DateTime.UtcNow;
                var roundedRemaining = TimeSpan.FromSeconds(Math.Ceiling(remaining.TotalSeconds));

                lock (_sync)
                {
                    _remaining = roundedRemaining;
                }

                TimeChanged?.Invoke(this, roundedRemaining);

                if (roundedRemaining.TotalSeconds <= 10 && roundedRemaining.TotalSeconds > 0)
                {
                    Last10Seconds?.Invoke(this, EventArgs.Empty);
                }

                var currentSecond = (int)roundedRemaining.TotalSeconds;
                if (currentSecond != lastSecond)
                {
                    lastSecond = currentSecond;
                    OnEverySecondActive(currentSecond);
                }

                if (DateTime.UtcNow >= _endTime)
                {
                    lock (_sync)
                    {
                        _remaining = TimeSpan.Zero;
                    }
                    CompleteSessionSuccessfully();
                    Stop();
                    break;
                }

                await Task.Delay(100, ct);
            }
        }
        // ------------------------------------------------------------------------------------
        #endregion



        #region Timer Utility Methods
        // ------------------------------------------------------------------------------------
        private void OnEverySecondActive(int remainingSeconds)
        {
            var totalSeconds = (int)_totalDuration.TotalSeconds;
            var activeSecondsElapsed = totalSeconds - remainingSeconds;

            var activeMinutes = activeSecondsElapsed / 60;

            if (activeMinutes > _activeMinutesLogged)
            {
                _activeMinutesLogged = activeMinutes;
                LogOneMinute();
            }
        }
        private void LogOneMinute()
        {

            _ = _learnSessionDbService.AddTimeToSessionAsync(_currentSession!, 1);
            _ = _userDbService.AddTimeToUser(1);

            if (_currentModule != null)
            {
                _ = _modulesDbService.AddTimeToModuleAsync(_currentModule, 1);
            }
            if (_currentDailyModuleStats != null)
            {
                _ = _dailyModuleStatsDbService.AddLearnedMinutesAsync(_currentDailyModuleStats, 1);
            }
        }

        private async void CompleteSessionSuccessfully()
        {
            if (_currentSession != null)
            {
                await _learnSessionDbService.CompleteSessionAsync(_currentSession, _currentDailyModuleStats);
            }
        }
        // ------------------------------------------------------------------------------------
        #endregion
    }
}