using AioStudy.Data.Interfaces;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class LearnSessionDbService
    {
        private readonly IRepository<LearnSession> _learnSessionRepository;
        private readonly DailyModuleStatsDbService _dailyModuleStatsDbService;

        public LearnSessionDbService(IRepository<LearnSession> learnSessionRepository, DailyModuleStatsDbService dailyModuleStatsDbService)
        {
            _learnSessionRepository = learnSessionRepository;
            _dailyModuleStatsDbService = dailyModuleStatsDbService;
        }

        public async Task<LearnSession?> CreateLearnSessionAsync(Module? module = null)
        {
            try
            {
                var learnSession = new LearnSession
                {
                    LearnedModuleId = module?.Id,
                    StartTime = DateTime.Now
                };
                return await _learnSessionRepository.CreateAsync(learnSession);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<LearnSession?> CreateLearnSessionIfNotExist(Module? module = null)
        {
            var sessions = await _learnSessionRepository.GetAllAsync();
            var existingSession = sessions.FirstOrDefault(s => s.EndTime == null);
            if (existingSession != null)
            {
                return existingSession;
            }
            return await CreateLearnSessionAsync(module);
        }

        public async Task AddTimeToSessionAsync(LearnSession session, int minutes)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            session.CurrentLearnedMinutes += minutes;
            await _learnSessionRepository.UpdateAsync(session);

        }

        public async Task CompleteSessionAsync(LearnSession session, DailyModuleStats? dailyModuleStats = null)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            session.SessionCompleted = true;
            session.EndTime = DateTime.Now;
            await _learnSessionRepository.UpdateAsync(session);
        }

        /// <summary>
        /// Set count to -1 to get all sessions
        /// </summary>
        /// <param name="count"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LearnSession>> GetRecentSessionsAsync(int count = 5, Module? module = null)
        {
            try
            {
                var sessions = await _learnSessionRepository.GetAllWithIncludesAsync("LearnedModule");
                var sessionList = sessions.ToList();

                if (module != null)
                {
                    sessionList = sessionList
                        .Where(s => s.LearnedModuleId == module.Id)
                        .ToList();
                }

                if (count == -1)
                {
                    return sessionList
                        .OrderByDescending(s => s.EndTime ?? s.StartTime)
                        .ToList();
                }

                return sessionList
                    .OrderByDescending(s => s.EndTime ?? s.StartTime)
                    .Take(count)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<LearnSession>();
            }
        }

        public async Task CancelSessionAsync(LearnSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            session.SessionCompleted = false; 
            session.EndTime = DateTime.Now;
            await _learnSessionRepository.UpdateAsync(session);
        }


        public async Task<IEnumerable<LearnSession>> GetSessionsByDateAsync(DateOnly date, Module? module = null)
        {
            var sessions = await _learnSessionRepository.GetAllWithIncludesAsync("LearnedModule");
            return sessions.Where(s => s.StartTime.Date == date.ToDateTime(TimeOnly.MinValue).Date && (module == null || s.LearnedModuleId == module.Id)).ToList();
        }

    }
}
