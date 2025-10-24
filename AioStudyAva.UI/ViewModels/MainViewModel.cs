using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AioStudy.Core.Manager.Settings;
using AioStudy.Core.Util;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace AioStudyAva.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        private string _currentViewName;
        

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public string CurrentViewName
        {
            get { return _currentViewName; }
            set
            {
                _currentViewName = value;
                OnPropertyChanged(nameof(CurrentViewName));
            }
        }

        public MainViewModel()
        {
            // Commands initialisieren
            

            // Standard View setzen
        }


        private void ExecuteDarkCommand(object? parameter)
        {
            var settingsManager = SettingsManager.Instance;
            
            settingsManager.Settings.Theme = Enums.ApplicationTheme.Dark.ToString();
            
            settingsManager.SaveSettings();
        }

        private void ExecuteLightCommand(object? parameter)
        {
            var settingsManager = SettingsManager.Instance;
            
            settingsManager.Settings.Theme = Enums.ApplicationTheme.Light.ToString();
            
            settingsManager.SaveSettings();
        }
    }
}
