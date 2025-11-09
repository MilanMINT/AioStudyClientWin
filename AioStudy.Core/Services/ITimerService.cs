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
        bool IsRunning { get; }

        void Start(TimeSpan duration, Module? module = null);
        void Stop();
        void Pause();
        void Resume();
        void Reset();

        event EventHandler<TimeSpan> TimeChanged;
        event EventHandler TimerEnded;
        event EventHandler<bool> RunningStateChanged;
        event EventHandler? TimerReset;
        DateTime? EndTime { get; }
    }
}
