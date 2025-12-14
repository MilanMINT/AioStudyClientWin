using AioStudy.Data.Interfaces;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class SemesterDbService
    {
        private readonly IRepository<Semester> _semesterRepository;
        private readonly IRepository<Module> _moduleRepository;

        public SemesterDbService(IRepository<Semester> semesterRepository, IRepository<Module> moduleRepository)
        {
            _semesterRepository = semesterRepository;
            _moduleRepository = moduleRepository;
        }

        public async Task<Semester> CreateSemesterAsync(Semester semester)
        {
            return await _semesterRepository.CreateAsync(semester);
        }

        public async Task<bool> DeleteSemester(int semesterId)
        {
            try
            {
                var semester = await _semesterRepository.GetByIdAsync(semesterId);
                if (semester == null)
                {
                    return false;
                }

                await _semesterRepository.DeleteAsync(semesterId);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> UpdateSemester(int semesterId, string name, DateTime startDate, DateTime endDate)
        {
            try
            {
                var semester = await _semesterRepository.GetByIdAsync(semesterId);
                if (semester == null)
                {
                    return false;
                }
                semester.Name = name;
                semester.StartDate = startDate;
                semester.EndDate = endDate;
                await _semesterRepository.UpdateAsync(semester);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Semester>> GetAllSemestersAsync()
        {
            return await _semesterRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Module>> GetModulesForSemester(Semester? semester = null)
        {
            var modules = await _moduleRepository.GetAllAsync();

            var allSemesters = await _semesterRepository.GetAllAsync();
            var semesterDict = allSemesters.ToDictionary(s => s.Id);

            foreach (var module in modules)
            {
                if (module.SemesterId.HasValue && semesterDict.TryGetValue(module.SemesterId.Value, out var sem))
                {
                    module.Semester = sem;
                }
            }

            if (semester == null)
            {
                return modules;
            }
            return modules.Where(m => m.SemesterId == semester.Id).ToList();
        }

        public async Task<int> GetModulesCountForSemester(Semester semester)
        {
            var modules = await _moduleRepository.GetAllAsync();
            return modules.Count(m => m.SemesterId == semester.Id);
        }

        public async Task<int> GetLearnedSemesterMinutes(Semester semester)
        {
            var modules = await _moduleRepository.GetAllAsync();
            if (modules == null || !modules.Any())
                return 0;
            return modules.Where(m => m.SemesterId == semester.Id).Sum(m => m.LearnedMinutes);
        }

        /// <summary>
        /// Chekc if the start date is valid (not overlapping with existing semesters)
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public async Task<bool> IsValidStartDate(DateTime startDate)
        {
            var semesters = await _semesterRepository.GetAllAsync();
            return !semesters.Any(s => s.StartDate <= startDate && s.EndDate >= startDate);
        }

        public async Task<Semester?> GetCurrentSemester()
        {
            var semesters = await _semesterRepository.GetAllAsync();
            var currentDate = DateTime.Now;
            return semesters.FirstOrDefault(s => s.StartDate <= currentDate && s.EndDate >= currentDate) ?? null;
        }

        public async Task<bool> IsCurrentSemester(Semester semester)
        {
            var currentSemester = await GetCurrentSemester();
            if (currentSemester == null)
                return false;
            return currentSemester.Id == semester.Id;
        }
    }
}
