using AioStudy.Data.Interfaces;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class DailyModuleStatsDbService
    {
        private readonly IRepository<DailyModuleStats> _dailyModuleStatsRepository;

        public DailyModuleStatsDbService(IRepository<DailyModuleStats> dailyModuleStatsRepository)
        {
            _dailyModuleStatsRepository = dailyModuleStatsRepository;
        }

        public async Task<DailyModuleStats> CreateDailyModuleStatIfNotExist(Module module)
        {
            var dailyModuleStats = await _dailyModuleStatsRepository.GetAllAsync();
            var todaysStat = dailyModuleStats
                                .Where(x => x.Date == DateTime.UtcNow.Date && x.ModuleId == module.Id)
                                .FirstOrDefault();

            if (todaysStat != null)
            {
                return todaysStat;
            }

            var newStats = new DailyModuleStats
            {
                ModuleId = module.Id,
                Date = DateTime.UtcNow.Date,
            };

            await _dailyModuleStatsRepository.CreateAsync(newStats);
            return newStats;
        }

        public async Task AddLearnedMinutesAsync(DailyModuleStats dailyModuleStats, int minutes)
        {
            dailyModuleStats.LearnedMinutes += minutes;
            await _dailyModuleStatsRepository.UpdateAsync(dailyModuleStats);
        }

        public async Task IncrementSessionCountAsync(DailyModuleStats dailyModuleStats)
        {
            dailyModuleStats.SessionsCount += 1;
            await _dailyModuleStatsRepository.UpdateAsync(dailyModuleStats);
        }
    }
}
