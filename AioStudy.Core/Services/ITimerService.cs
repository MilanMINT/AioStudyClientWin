using AioStudy.Core.Util;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Services
{
    public interface ITimerService
    {
        TimeSpan Remaining { get; }
        DateTime EndTime { get; }

        bool IsRunning { get; }
        bool IsPaused { get; }
        bool IsBreak { get; }

        void Start(TimeSpan duration, Module? module = null);
        void Stop();
        void Pause();
        void Resume();
        void Reset();
        void StartBreak(Enums.TimerBreakType breakType);
        void EndBreak();

        event EventHandler<TimeSpan> TimeChanged;
        event EventHandler TimerEnded;
        event EventHandler<bool> RunningStateChanged;
        event EventHandler<bool> PausedStateChanged;
        event EventHandler BreakEnded;
        event EventHandler<Enums.TimerBreakType> BreakStateChanged;
        event EventHandler Last10Seconds;
    }
}
