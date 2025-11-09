using AioStudy.Core.Data.Services;
using AioStudy.Core.Services;
using AioStudy.Models;
using AioStudy.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AioStudy.UI.WpfServices
{
    public class TimerService : ITimerService, IDisposable
    {
        private CancellationTokenSource? _cts;
        private DateTime _endTime;
        private TimeSpan _remaining;
        private readonly object _sync = new();

        private TimeSpan _initialDuration;
        public TimeSpan Remaining => _remaining;
        public bool IsRunning { get; private set; }
        public DateTime? EndTime { get; private set; }

        public event EventHandler<TimeSpan>? TimeChanged;
        public event EventHandler? TimerEnded;
        public event EventHandler<bool>? RunningStateChanged;
        public event EventHandler? TimerReset;

        public Module? SelectedModule { get; set; } = null;

        private const int PollIntervalMs = 200;
        private const int PollsPerMinute = (1000 / PollIntervalMs) * 60;
        private int pollCounter = 0;

        private UserDbService _userDbService;
        private ModulesDbService _modulesDbService;

        public TimerService(UserDbService userDbService, ModulesDbService modulesDbService)
        {
            _userDbService = userDbService;
            _modulesDbService = modulesDbService;
        }

        public void Start(TimeSpan duration, Module? module = null)
        {
            lock (_sync)
            {
                _initialDuration = duration;
                StopInternal();
                _remaining = RoundToSeconds(duration);
                _endTime = DateTime.UtcNow.Add(duration);
                EndTime = DateTime.Now.Add(duration);
                _cts = new CancellationTokenSource();
                IsRunning = true;
                SelectedModule = module;
                pollCounter = 0;
                OnRunningChanged(true);
                OnTimeChanged(_remaining);


                _ = RunLoopAsync(_cts.Token);
            }
        }

        public void Stop()
        {
            lock (_sync)
            {
                StopInternal();
                _remaining = TimeSpan.Zero;
                EndTime = null;
                pollCounter = 0;
                OnTimeChanged(_remaining);
            }
        }

        public void Pause()
        {
            lock (_sync)
            {
                if (!IsRunning) return;
                var rawRemaining = _endTime - DateTime.UtcNow;
                if (rawRemaining < TimeSpan.Zero) rawRemaining = TimeSpan.Zero;

                _remaining = RoundToSeconds(rawRemaining);

                StopInternal();
                OnTimeChanged(_remaining);
            }
        }

        public void Resume()
        {
            lock (_sync)
            {
                if (IsRunning || _remaining <= TimeSpan.Zero) return;
                _endTime = DateTime.UtcNow.Add(_remaining);
                EndTime = DateTime.Now.Add(_remaining);
                _cts = new CancellationTokenSource();
                IsRunning = true;
                OnRunningChanged(true);
                OnTimeChanged(_remaining);
                _ = RunLoopAsync(_cts.Token);
            }
        }

        private async Task RunLoopAsync(CancellationToken ct)
        {
            try
            {
                var delay = TimeSpan.FromMilliseconds(PollIntervalMs);
                while (!ct.IsCancellationRequested)
                {
                    var rem = _endTime - DateTime.UtcNow;

                    pollCounter++;

                    if (pollCounter >= PollsPerMinute)
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _userDbService.AddTimeToUser(1);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Fehler beim Speichern der Zeit: {ex.Message}");
                            }
                        });
                        pollCounter = 0;
                    }

                    if (rem <= TimeSpan.Zero)
                    {
                        _ = DispatchAsync(() =>
                        {
                            _remaining = TimeSpan.Zero;
                            OnTimeChanged(_remaining);
                        });

                        lock (_sync)
                        {
                            StopInternal();
                            EndTime = null;
                        }

                        OnTimerEnded();
                        break;
                    }

                    var roundedRem = RoundToSeconds(rem);
                    _ = DispatchAsync(() =>
                    {
                        _remaining = roundedRem;
                        OnTimeChanged(_remaining);
                    });

                    try
                    {
                        await Task.Delay(delay, ct);
                    }
                    catch (OperationCanceledException) { break; }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler in RunLoopAsync: {ex.Message}");
            }
        }


        private void StopInternal()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            IsRunning = false;
            OnRunningChanged(false);
        }

        private Task DispatchAsync(Action action)
        {
            var app = Application.Current;
            if (app == null || app.Dispatcher == null || app.Dispatcher.HasShutdownStarted)
            {
                action();
                return Task.CompletedTask;
            }

            return app.Dispatcher.InvokeAsync(action).Task;
        }

        private void OnTimeChanged(TimeSpan ts) => TimeChanged?.Invoke(this, ts);
        private void OnTimerEnded() => TimerEnded?.Invoke(this, EventArgs.Empty);
        private void OnRunningChanged(bool running) => RunningStateChanged?.Invoke(this, running);

        public void Dispose()
        {
            StopInternal();
        }

        public void Reset()
        {
            lock (_sync)
            {
                StopInternal();

                _remaining = RoundToSeconds(_initialDuration);
                _endTime = DateTime.UtcNow.Add(_initialDuration);
                EndTime = DateTime.Now.Add(_initialDuration);

                pollCounter = 0;

                OnTimeChanged(_remaining);
                OnTimerReset();
            }
        }

        protected virtual void OnTimerReset()
        {
            TimerReset?.Invoke(this, EventArgs.Empty);
        }

        private TimeSpan RoundToSeconds(TimeSpan time)
        {
            return TimeSpan.FromSeconds(Math.Ceiling(time.TotalSeconds));
        }
    }
}