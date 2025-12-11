using AioStudy.Core.Data.Services;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Util.Modules
{
    public static class ModuleHelper
    {
        public static class Maths
        {
            public static TimeSpan CalculateAverageSessionTime(IEnumerable<LearnSession> sessions)
            {
                if (sessions == null || !sessions.Any())
                {
                    return TimeSpan.Zero;
                }

                var validSessions = sessions
                    .Where(s => s.CurrentLearnedMinutes > 0)
                    .ToList();

                if (!validSessions.Any())
                {
                    return TimeSpan.Zero;
                }

                double totalMinutes = validSessions.Sum(s =>
                {
                    if (s.CurrentLearnedMinutes > 0)
                    {
                        return s.CurrentLearnedMinutes;
                    }

                    return Math.Max(0, s.GetTotalDuration().TotalMinutes);
                });

                double averageMinutes = totalMinutes / validSessions.Count;
                return TimeSpan.FromMinutes(averageMinutes);
            }
        }

    }
}
