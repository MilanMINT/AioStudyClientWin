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

        public SemesterDbService(IRepository<Semester> semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<Semester> CreateSemesterAsync(string name, DateTime startDate, DateTime endDate)
        {
            var semester = new Semester(name, startDate, endDate);
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
    }
}
