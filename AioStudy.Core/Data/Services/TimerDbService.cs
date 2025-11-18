using AioStudy.Data.Interfaces;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class TimerDbService
    {
        public readonly IRepository<Semester> _semesterRepository;
        public readonly IRepository<Module> _moduleRepository;
        public readonly IRepository<DailyModuleStats> _dailyModuleStatsRepository;
        public readonly IRepository<LearnSession> _learnSessionRepository;
        public readonly IRepository<User> _userRepository;

        public TimerDbService(
            IRepository<Semester> semesterRepository,
            IRepository<Module> moduleRepository,
            IRepository<DailyModuleStats> dailyModuleStatsRepository,
            IRepository<LearnSession> learnSessionRepository,
            IRepository<User> userRepository)
        {
            _semesterRepository = semesterRepository;
            _moduleRepository = moduleRepository;
            _dailyModuleStatsRepository = dailyModuleStatsRepository;
            _learnSessionRepository = learnSessionRepository;
            _userRepository = userRepository;
        }
    }
}
