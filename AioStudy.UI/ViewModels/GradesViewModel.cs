using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels
{
    public class GradesViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;

        public GradesViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
