using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AioStudy.UI.ViewModels.Components
{
    public class ModuleCardViewModel : ViewModelBase
    {
        private Module _module;

        public Module Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
                OnPropertyChanged(nameof(LearnedMinutesString));
            }
        }

        public string LearnedMinutesString
        {
            get
            {
                int totalMinutes = _module?.LearnedMinutes ?? 0;
                var ts = TimeSpan.FromMinutes(totalMinutes);
                int hours = (int)ts.TotalHours;
                int minutes = ts.Minutes;
                return $"{hours}h {minutes}m";
            }
            set
            {
                if (_module != null)
                {
                    if (TimeSpan.TryParse(value, out var ts))
                    {
                        _module.LearnedMinutes = (int)ts.TotalMinutes;
                    }
                    OnPropertyChanged(nameof(LearnedMinutesString));
                }
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand? EditCommand { get; }

        public ModuleCardViewModel(Module module, ICommand deleteCommand, ICommand editCommand = null)
        {
            Module = module;
            DeleteCommand = deleteCommand;
            EditCommand = editCommand;
            EditCommand = editCommand;
        }
    }
}
