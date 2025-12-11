using AioStudy.Models;
using AioStudy.UI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.UI.ViewModels.Overview
{
    public class ModuleStatisticsOverviewViewmodel : ViewModelBase
    {
        private Module _module;
        private ModuleOverViewViewModel _moduleOverViewViewModel;
        private MainViewModel _mainViewModel;


        private string _moduleName;

        public string ModuleName
        {
            get { return _moduleName; }
            set
            {
                _moduleName = value;
                OnPropertyChanged(nameof(ModuleName));
            }
        }

        public RelayCommand BackCommand { get; }

        public ModuleStatisticsOverviewViewmodel(Module module, ModuleOverViewViewModel moduleOverViewViewModel, MainViewModel mainViewModel)
        {
            _module = module;
            _moduleOverViewViewModel = moduleOverViewViewModel;
            _mainViewModel = mainViewModel;

            ModuleName = module.Name;

            BackCommand = new RelayCommand(ExecuteBackCommand);
        }


        // NEEDED:
        // 


        private void ExecuteBackCommand(object? obj)
        {
            _mainViewModel.CurrentViewModel = _moduleOverViewViewModel;
            _mainViewModel.CurrentViewName = $"{_module.Name}´s Overview";
        }
    }
}
