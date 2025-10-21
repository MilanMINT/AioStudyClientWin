using AioStudy.Core.Data.Services;
using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels
{
    public class ModulesViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;
        private readonly ModulesDbService _modulesDbService;

        private string _semesterName;

        private Semester _selectedSemester;

        public string SemesterName
        {
            get { return _semesterName; }
            set
            {
                _semesterName = value;
                OnPropertyChanged(nameof(SemesterName));
            }
        }

        public ModulesViewModel(ModulesDbService modulesDbService)
        {
            _modulesDbService = modulesDbService;
        }

        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
