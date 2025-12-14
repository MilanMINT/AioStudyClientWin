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

        /// <summary>
        /// Returns true if the User table is empty, false otherwise.
        /// </summary>
        /// <returns></returns>
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
    }
}
