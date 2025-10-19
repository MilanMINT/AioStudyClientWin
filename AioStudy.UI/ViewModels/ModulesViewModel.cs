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
        private readonly MainViewModel _mainViewModel;
        private readonly SemesterViewModel _semesterViewModel;

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

        public ModulesViewModel(MainViewModel mainViewModel, SemesterViewModel semesterViewModel)
        {
            _mainViewModel = mainViewModel;
            _semesterViewModel = semesterViewModel;
        }

        public void SetSelectedSemester(Semester semester)
        {
            _selectedSemester = semester;

            if (semester != null)
            {
                SemesterName = $"Module für: {semester.Name}";
            }
            else
            {
                SemesterName = "Kein Semester ausgewählt";
            }
        }
    }
}
