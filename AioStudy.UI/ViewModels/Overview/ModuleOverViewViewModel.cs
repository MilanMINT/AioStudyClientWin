using AioStudy.Core.Data.Services;
using AioStudy.Models;
using AioStudy.UI.Commands;
using AioStudy.UI.WpfServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AioStudy.UI.ViewModels.Overview
{
    public class ModuleOverViewViewModel : ViewModelBase
    {
        private Module _module;
        private ModulesDbService _modulesDbService;
        private ModulesViewModel _modulesViewModel;
        private MainViewModel _mainViewModel;

        public Module Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
                OnPropertyChanged(nameof(ModuleName));
            }
        }

        public string ModuleName
        {
            get { return _module?.Name ?? "Unknown Module"; }
            set
            {
                if (_module != null)
                {
                    _module.Name = value;
                    OnPropertyChanged(nameof(ModuleName));
                }
            }
        }

        public RelayCommand BackCommand { get; }
        public RelayCommand DeleteModuleCommand { get; }
        public RelayCommand EditModuleCommand { get; }

        public ModuleOverViewViewModel(Module module, ModulesViewModel modulesViewModel, MainViewModel mainViewModel, RelayCommand deleteModuleCommand)
        {
            Module = module;
            _modulesDbService = App.ServiceProvider.GetRequiredService<ModulesDbService>();
            _modulesViewModel = modulesViewModel;
            _mainViewModel = mainViewModel;
            
            // Wrapper Command für Delete mit Navigation
            DeleteModuleCommand = new RelayCommand(async parameter => await DeleteModuleWithNavigation(parameter));
            
            // Commands
            BackCommand = new RelayCommand(ExecuteBackCommand);
        }

        private async Task DeleteModuleWithNavigation(object? parameter)
        {
            if (parameter is Module module)
            {
                bool confirmed = ConfirmModalService.ShowDeleteConfirmation(module.Name);

                if (confirmed)
                {
                    try
                    {
                        bool success = await _modulesDbService.DeleteModule(module.Id);
                        if (success)
                        {
                            // Remove from ModulesViewModel collection
                            _modulesViewModel.Modules.Remove(module);
                            
                            // Show success message
                            await ToastService.ShowSuccessAsync("Module Deleted!", $"The module '{module.Name}' has been successfully deleted.");
                            
                            // Navigate back to modules view
                            _mainViewModel.CurrentViewModel = _modulesViewModel;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Fehler beim Löschen des Moduls: {ex.Message}");
                    }
                }
            }
        }

        private void ExecuteBackCommand(object? obj)
        {
            _mainViewModel.CurrentViewModel = _modulesViewModel;
        }
    }
}
