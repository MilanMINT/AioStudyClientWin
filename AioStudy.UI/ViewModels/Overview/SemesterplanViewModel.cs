using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Overview
{
    public class SemesterplanViewModel : ViewModelBase
    {
        private SemesterViewModel _semesterViewModel;
        private MainViewModel _mainViewModel;

        public SemesterplanViewModel(SemesterViewModel semesterViewModel, MainViewModel mainViewModel)
        {
            _semesterViewModel = semesterViewModel;
            _mainViewModel = mainViewModel;
        }
    }
}
