using AioStudy.Data.Interfaces;
using AioStudy.Models;
using AioStudy.Models.DailyPlannerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class DailyPlanDbService
    {
        private readonly IRepository<DailyPlan> _dailyPlanRepository;

        public DailyPlanDbService(IRepository<DailyPlan> dailyPlanRepository)
        {
            _dailyPlanRepository = dailyPlanRepository;
        }   

        public async Task<DailyPlan> InitDailyPlan()
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                var existingPlans = await _dailyPlanRepository.GetAllAsync();
                var todayPlan = existingPlans.FirstOrDefault(dp => dp.Date == today);
                if (todayPlan != null)
                {
                    return todayPlan;
                }
                var newDailyPlan = new DailyPlan
                {
                    Date = today
                };
                var createdPlan = await _dailyPlanRepository.CreateAsync(newDailyPlan);
                return createdPlan ?? throw new Exception("Failed to create DailyPlan");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckIfPlanAlreadyExist(DateOnly date)
        {
            try
            {
                var existingPlans = await _dailyPlanRepository.GetAllAsync();
                return existingPlans.Any(dp => dp.Date == date);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DailyPlan> CreateDailyPlan(DateOnly date)
        {
            try
            {
                var newDailyPlan = new DailyPlan
                {
                    Date = date
                };
                var createdPlan = await _dailyPlanRepository.CreateAsync(newDailyPlan);
                return createdPlan ?? throw new Exception("Failed to create DailyPlan");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DailyPlan?> GetDailyPlanByDate(DateOnly date)
        {
            try
            {
                var existingPlans = await _dailyPlanRepository.GetAllAsync();
                return existingPlans.FirstOrDefault(dp => dp.Date == date);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
