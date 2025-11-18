using AioStudy.Core.Services;
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
        private DateTime _endTime;
        private TimeSpan _remaining;

        public TimeSpan Remaining => _remaining;
        public DateTime EndTime => _endTime;

        private bool _isRunning;
        public bool IsRunning => _isRunning;

        private bool _isPaused = false;
        public bool IsPaused => _isPaused;

        private readonly object _sync = new();

        public event EventHandler<TimeSpan> TimeChanged;
        public event EventHandler TimerEnded;
        public event EventHandler<bool> RunningStateChanged;
        /// <summary>
        /// True if paused, false if resumed
        /// </summary>
        public event EventHandler<bool> PausedStateChanged;

        private DateTime _startTime;
        private TimeSpan _totalDuration;
        private int _lastMinute = 0;

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

        public void Start(TimeSpan duration, Module? module = null)
        {
            _remaining = duration;
            _totalDuration = duration;
            _startTime = DateTime.UtcNow;
            _endTime = DateTime.UtcNow.Add(duration);
            _cts = new CancellationTokenSource();
            _isRunning = true;
            _lastMinute = 0;
            RunningStateChanged?.Invoke(this, true);
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


        // TODO ADD DB LOGIC; UI STUFF

        public void Reset()
        {
            _isRunning = false;
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
                TimeChanged?.Invoke(this, TimeSpan.Zero);
                TimerEnded?.Invoke(this, EventArgs.Empty);
                RunningStateChanged?.Invoke(this, false);
                _currentDailyModuleStats = null;
                _isRunning = false;
                TimeChanged?.Invoke(this, _totalDuration);
            }
        }

        private TimeSpan __roundedRemaining;

        private async Task RunLoopAsync(CancellationToken ct)
        {
            var lastSecond = -1;

            while (!ct.IsCancellationRequested)
            {
                bool paused;
                lock (_sync) { paused = _isPaused; }

                if (paused)
                {
                    await Task.Delay(100, ct);
                    continue;
                }

                var __remaining = _endTime - DateTime.UtcNow;
                __roundedRemaining = TimeSpan.FromSeconds(Math.Ceiling(__remaining.TotalSeconds));

                lock (_sync)
                {
                    _remaining = __roundedRemaining;
                }

                TimeChanged?.Invoke(this, __roundedRemaining);

                var currentSecond = (int)__roundedRemaining.TotalSeconds;
                if (currentSecond != lastSecond)
                {
                    lastSecond = currentSecond;
                    OnEverySecond(currentSecond);
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

        private void OnEverySecond(int remainingSeconds)
        {
            var elapsed = DateTime.UtcNow - _startTime;
            var elapsedMinutes = (int)elapsed.TotalMinutes;

            // Code im If wird jede Minute ausgeführt
            if (elapsedMinutes > _lastMinute)
            {
                OnEveryMinute(elapsedMinutes);
            }

            System.Diagnostics.Debug.WriteLine($"[Timer] Tick: {remainingSeconds} seconds remaining (Elapsed: {elapsed.TotalSeconds:F1}s)");
        }

        private void OnEveryMinute(int elapsedMinutes)
        {
            _lastMinute = elapsedMinutes;
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
            System.Diagnostics.Debug.WriteLine($"[Timer] ✅ {elapsedMinutes} minute(s) passed.");
        }

        private async void CompleteSessionSuccessfully()
        {
            if (_currentSession != null)
            {
                await _learnSessionDbService.CompleteSessionAsync(_currentSession, _currentDailyModuleStats);
            }
        }
    }
}