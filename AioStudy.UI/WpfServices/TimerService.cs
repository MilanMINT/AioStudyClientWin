using AioStudy.Core.Services;
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


        private const int PollIntervalMs = 200;

        public TimerService()
        {
        }

        public void Start(TimeSpan duration)
        {
            lock (_sync)
            {
                _initialDuration = duration;
                StopInternal();
                _remaining = duration;
                _endTime = DateTime.UtcNow.Add(duration);
                EndTime = DateTime.Now.Add(duration);
                _cts = new CancellationTokenSource();
                IsRunning = true;
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
                OnTimeChanged(_remaining);
            }
        }

        public void Pause()
        {
            lock (_sync)
            {
                if (!IsRunning) return;
                _remaining = _endTime - DateTime.UtcNow;
                if (_remaining < TimeSpan.Zero) _remaining = TimeSpan.Zero;
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
                    if (rem <= TimeSpan.Zero)
                    {
                        await DispatchAsync(() =>
                        {
                            _remaining = TimeSpan.Zero;
                            OnTimeChanged(_remaining);
                            StopInternal();
                            EndTime = null;
                            OnTimerEnded();
                        });
                        break;
                    }

                    await DispatchAsync(() => { _remaining = rem; OnTimeChanged(_remaining); });

                    try
                    {
                        await Task.Delay(delay, ct);
                    }
                    catch (OperationCanceledException) { break; }
                }
            }
            catch (OperationCanceledException) { }
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

                _remaining = _initialDuration;
                _endTime = DateTime.UtcNow.Add(_initialDuration);
                EndTime = DateTime.Now.Add(_initialDuration);

                OnTimeChanged(_remaining);
                OnTimerReset();
            }
        }

        protected virtual void OnTimerReset()
        {
            TimerReset?.Invoke(this, EventArgs.Empty);
        }
    }
}