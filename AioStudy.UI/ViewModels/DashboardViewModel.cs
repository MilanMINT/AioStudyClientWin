using AioStudy.Core.Services;
using AioStudy.UI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {

        private MainViewModel _mainViewModel;


        public DashboardViewModel()
        {
        }


        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
