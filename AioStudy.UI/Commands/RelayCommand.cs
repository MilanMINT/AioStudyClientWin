using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AioStudy.UI.Commands
{
    public class RelayCommand : ICommand
    {
        private Action<object?> execute;
        private Func<object?, bool>? canExecute;

        private event EventHandler? canExecuteChangedInternal;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; canExecuteChangedInternal += value; }
            remove { CommandManager.RequerySuggested -= value; canExecuteChangedInternal -= value; }
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canexecute = null)
        {
            this.execute = execute;
            this.canExecute = canexecute;
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            this.execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            canExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
        }
    }
}
