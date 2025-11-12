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

        private readonly UserDbService _userDbService;
        private readonly ModulesDbService _modulesDbService;
        private readonly LearnSessionDbService _learnSessionDbService;

        private CancellationTokenSource? _cts;

        public TimerService(UserDbService userDbService, ModulesDbService modulesDbService, LearnSessionDbService learnSessionDbService)
        {
            _userDbService = userDbService;
            _modulesDbService = modulesDbService;
            _learnSessionDbService = learnSessionDbService;
        }

        public void Start(TimeSpan duration, Module? module = null)
        {
            _remaining = duration;
            _endTime = DateTime.UtcNow.Add(duration);
            _cts = new CancellationTokenSource();
            _isRunning = true;
            RunningStateChanged?.Invoke(this, true);
            _ = RunLoopAsync(_cts.Token);
        }

        public void Pause()
        {
            _isPaused = true;
            PausedStateChanged?.Invoke(this, true);
        }

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
                _isRunning = false;
            }
        }

        private async Task RunLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                bool paused;
                //Lock for Thread Safety
                lock (_sync) { paused = _isPaused; }

                if (paused)
                {
                    await Task.Delay(100, ct);
                    continue;
                }

                if (DateTime.UtcNow >= _endTime)
                {
                    lock (_sync)
                    {
                        _remaining = TimeSpan.Zero;
                    }
                    Stop();
                    break;
                }

                var __remaining = _endTime - DateTime.UtcNow;
                var __roundedRemaining = TimeSpan.FromSeconds(Math.Ceiling(__remaining.TotalSeconds));
                lock (_sync)
                {
                    _remaining = __roundedRemaining;
                }
                TimeChanged?.Invoke(this, __roundedRemaining);
                await Task.Delay(100, ct);
            }
        }
    }
}
