using AioStudy.Data.Interfaces;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Data.Services
{
    public class ModulesDbService
    {
        private readonly IRepository<Module> _moduleRepository;

        public ModulesDbService(IRepository<Module> moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        public async Task<Module?> CreateModuleAsync(Module module)
        {
            try
            {
                return await _moduleRepository.CreateAsync(module);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteModule(int moduleId)
        {
            try
            {
                var module = await _moduleRepository.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return false;
                }

                await _moduleRepository.DeleteAsync(moduleId);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> UpdateModule(int moduleId, string name)
        {
            try
            {
                var module = await _moduleRepository.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return false;
                }
                module.Name = name;
                await _moduleRepository.UpdateAsync(module);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateModuleAsync(Module module)
        {
            if (module == null) return false;

            try
            {
                var existing = await _moduleRepository.GetByIdAsync(module.Id);
                if (existing == null)
                    return false;

                existing.Name = module.Name;
                existing.ExamDate = module.ExamDate;
                existing.ExamStatus = module.ExamStatus;
                existing.Grade = module.Grade;
                existing.ModuleCredits = module.ModuleCredits;
                existing.Color = module.Color;
                existing.SemesterId = module.SemesterId;
                existing.LearnedMinutes = module.LearnedMinutes;
                existing.ModuleAttempt = module.ModuleAttempt;

                // Persist
                await _moduleRepository.UpdateAsync(existing);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<IEnumerable<Module>> GetAllModulesAsync()
        {
            return await _moduleRepository.GetAllAsync();
        }

    }
}
