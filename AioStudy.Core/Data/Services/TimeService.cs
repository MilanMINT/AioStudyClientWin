using AioStudy.Core.Services;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class TimeService : ITimerService, IDisposable
    {
        private CancellationTokenSource? _cts;
        private DateTime _endTime;
        private TimeSpan _remaining;
        private readonly object _sync = new();

        private TimeSpan _initialDuration;
        public TimeSpan Remaining
        {
            get
            {
                lock (_sync)
                {
                    return _remaining;
                }
            }
        }

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

        private readonly UserDbService _userDbService;
        private readonly ModulesDbService _modulesDbService;
        private readonly LearnSessionDbService _learnSessionDbService;

        private LearnSession? _currentSession;
        private bool _sessionCreated = false;

        public TimeSpan InitDateTime { get; set; }

        public TimeService(
            UserDbService userDbService,
            ModulesDbService modulesDbService,
            LearnSessionDbService learnSessionDbService)
        {
            _userDbService = userDbService;
            _modulesDbService = modulesDbService;
            _learnSessionDbService = learnSessionDbService;
        }

        public void Start(TimeSpan duration, Module? module = null)
        {
            lock (_sync)
            {
                System.Diagnostics.Debug.WriteLine($"STARTED!!!!!!!!!!!!!!!!!!!!!!!");

                InitDateTime = duration;
                StopInternal();
                _remaining = RoundToSeconds(duration);
                _endTime = DateTime.UtcNow.Add(duration);
                EndTime = DateTime.Now.Add(duration);
                _cts = new CancellationTokenSource();
                IsRunning = true;
                SelectedModule = module;
                pollCounter = 0;
                _sessionCreated = false;
                _currentSession = null;
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
                _sessionCreated = false;
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

                    if (rem <= TimeSpan.Zero)
                    {
                        LearnSession? sessionToComplete;
                        lock (_sync)
                        {
                            sessionToComplete = _currentSession;
                        }

                        if (sessionToComplete != null && pollCounter > 0)
                        {
                            int remainingMinutes = (int)Math.Ceiling(pollCounter / (double)PollsPerMinute);

                            try
                            {
                                await _userDbService.AddTimeToUser(remainingMinutes);
                                await _learnSessionDbService.AddTimeToSessionAsync(sessionToComplete, remainingMinutes);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Fehler beim Speichern der finalen Zeit: {ex.Message}");
                            }
                        }

                        if (sessionToComplete != null)
                        {
                            try
                            {
                                await _learnSessionDbService.CompleteSessionAsync(sessionToComplete);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Fehler beim Abschließen der Session: {ex.Message}");
                            }
                        }

                        lock (_sync)
                        {
                            _remaining = TimeSpan.Zero;
                        }
                        OnTimeChanged(TimeSpan.Zero);

                        lock (_sync)
                        {
                            StopInternal();
                            SelectedModule = null;
                            _currentSession = null;
                            _sessionCreated = false;
                            EndTime = null;
                        }

                        OnTimerEnded();
                        break;
                    }
                    System.Diagnostics.Debug.WriteLine($"PollCOunter {pollCounter}");
                    pollCounter++;

                    if (pollCounter >= PollsPerMinute)
                    {
                        if (!_sessionCreated)
                        {
                            _sessionCreated = true;
                            try
                            {
                                var session = await _learnSessionDbService.CreateLearnSessionAsync(SelectedModule);
                                if (session != null)
                                {
                                    lock (_sync)
                                    {
                                        _currentSession = session;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Fehler beim Erstellen der LearnSession: {ex.Message}");
                            }
                        }

                        try
                        {
                            await _userDbService.AddTimeToUser(1);

                            LearnSession? currentSessionSnapshot;
                            lock (_sync)
                            {
                                currentSessionSnapshot = _currentSession;
                            }

                            if (currentSessionSnapshot != null)
                            {
                                await _learnSessionDbService.AddTimeToSessionAsync(currentSessionSnapshot, 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Fehler beim Speichern der Zeit: {ex.Message}");
                        }

                        pollCounter = 0;
                    }

                    var roundedRem = RoundToSeconds(rem);

                    lock (_sync)
                    {
                        _remaining = roundedRem;
                    }
                    OnTimeChanged(roundedRem);

                    try
                    {
                        await Task.Delay(delay, ct);
                    }
                    catch (OperationCanceledException) { break; }
                }
            }
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
                //StopInternal();
                //SelectedModule = null;

                //_remaining = TimeSpan.Zero;
                //_endTime = DateTime.UtcNow.Add(_initialDuration);
                //EndTime = DateTime.Now.Add(_initialDuration);

                //_sessionCreated = false;
                //_currentSession = null;
                //pollCounter = 0;

                //OnTimerReset();

                Stop();
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