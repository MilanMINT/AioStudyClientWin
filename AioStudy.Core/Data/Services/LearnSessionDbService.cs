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
            catch (Exception ex)
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

        public async Task<List<LearnSession>> GetRecentSessionsAsync(int count = 5)
        {
            try
            {
                var sessions = await _learnSessionRepository.GetAllWithIncludesAsync("LearnedModule");
                var sessionList = sessions.ToList();

                return sessionList
                    .OrderByDescending(s => s.EndTime ?? s.StartTime)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
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

        public async Task<IEnumerable<LearnSession>> GetSessionsByModule(Module module)
        {
            if (module == null) return Enumerable.Empty<LearnSession>();
            var sessions = await _learnSessionRepository.GetAllAsync();
            var moduleSessions = sessions.Where(s => s.LearnedModuleId == module.Id).ToList();
            return moduleSessions;
        }
    }
}
