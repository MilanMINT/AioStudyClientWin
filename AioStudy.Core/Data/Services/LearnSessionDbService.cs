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

        public LearnSessionDbService(IRepository<LearnSession> learnSessionRepository)
        {
            _learnSessionRepository = learnSessionRepository;
        }

        public async Task<LearnSession?> CreateLearnSessionAsync(Module? module = null)
        {
            try
            {
                var learnSession = new LearnSession
                {
                    LearnedModule = module,
                    StartTime = DateTime.UtcNow
                };
                return await _learnSessionRepository.CreateAsync(learnSession);
            }
            catch (Exception)
            {
                return null;
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

        public async Task CompleteSessionAsync(LearnSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            session.SessionCompleted = true;
            session.EndTime = DateTime.UtcNow;
            await _learnSessionRepository.UpdateAsync(session);
        }
    }
}
