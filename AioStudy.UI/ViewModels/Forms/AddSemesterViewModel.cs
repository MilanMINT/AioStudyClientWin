using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Forms
{
    public class AddSemesterViewModel : ViewModelBase
    {
        private readonly SemesterViewModel _semesterViewModel;

        public AddSemesterViewModel(SemesterViewModel semesterViewModel)
        {
            _semesterViewModel = semesterViewModel;
        }
    }
}
