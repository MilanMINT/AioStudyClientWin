using AioStudy.Data.Interfaces;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class QuickTimerDbService 
    {
        private readonly IRepository<QuickTimer> _quickTimerRepository;

        public QuickTimerDbService(IRepository<QuickTimer> quickTimerRepository)
        {
            _quickTimerRepository = quickTimerRepository;
        }

        public async Task<QuickTimer?> CreateQuickTimerAsync(QuickTimer quickTimer)
        {
            try
            {
                return await _quickTimerRepository.CreateAsync(quickTimer);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<QuickTimer>?> GetAllQuickTimers()
        {
            try
            {
                return await _quickTimerRepository.GetAllAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<QuickTimer?> UpdateTimerAsync(QuickTimer quickTimer)
        {
            try
            {
                await _quickTimerRepository.UpdateAsync(quickTimer);
                return quickTimer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteQuickTimer(QuickTimer quickTimer)
        {
            try
            {
                await _quickTimerRepository.DeleteAsync(quickTimer.Id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
