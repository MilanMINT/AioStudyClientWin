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
        public static class Math
        {
            public static TimeSpan CalculateAverageSessionTime(IEnumerable<LearnSession> sessions)
            {
                if (sessions == null || !sessions.Any())
                {
                    return TimeSpan.Zero;
                }
                double totalMinutes = sessions.Sum(s => s.CurrentLearnedMinutes);
                double averageMinutes = totalMinutes / sessions.Count();
                return TimeSpan.FromMinutes(averageMinutes);
            }
        }

    }
}
