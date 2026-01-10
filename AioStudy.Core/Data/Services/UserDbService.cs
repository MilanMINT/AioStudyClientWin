using AioStudy.Data.Interfaces;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class UserDbService
    {
        public readonly IRepository<User> _userRepository;

        public UserDbService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUser()
        {
            var users = await _userRepository.GetAllWithIncludesAsync(nameof(User.CurrentSemester));
            return users.FirstOrDefault();
        }

        public async Task<bool> IsUserTableEmpty()
        {
            var users = await _userRepository.GetAllAsync();
            return !users.Any();
        }

        public async Task<User?> CreateUserAsync(User user)
        {
            try
            {
                return await _userRepository.CreateAsync(user);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task AddTimeToUser(int minutes)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault();
            if (user != null)
            {
                user.LearnedMinutes += minutes;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<bool> SetCurrentSemester(Semester semester)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault();
            if (user != null)
            {
                user.CurrentSemester = semester;
                await _userRepository.UpdateAsync(user);
                return true;
            }
            return false;
        }

        public async Task<int> GetCurrentStreak()
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault();
            if (user != null && user.LearningStreak != null)
            {
                return user.LearningStreak.Value;
            }
            return 0;
        }


        public async Task<int?> UpdateLearningStreakOnLoginAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault();
            if (user == null) return null;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var yesterday = today.AddDays(-1);

            if (user.LastLoggedIn == today)
            {
                user.LearningStreak ??= 1;
                await _userRepository.UpdateAsync(user);
                return user.LearningStreak;
            }

            if (user.LastLoggedIn == yesterday)
            {
                user.LearningStreak = (user.LearningStreak ?? 0) + 1;
            }
            else
            {
                user.LearningStreak = 1;
            }

            user.LastLoggedIn = today;
            await _userRepository.UpdateAsync(user);
            return user.LearningStreak;
        }

        public async Task CheckLearningStreak()
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault();
            if (user == null) return;
            if (user.LastLoggedIn != null)
            {
                DateOnly yesterday = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                if (user.LastLoggedIn < yesterday)
                {
                    user.LearningStreak = 0;
                    await _userRepository.UpdateAsync(user);
                }
            }
        }
    }
}
