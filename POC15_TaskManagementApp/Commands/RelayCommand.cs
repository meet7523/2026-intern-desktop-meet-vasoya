using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POC15__TaskManagementApp.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _executeWithParameter;

        private readonly Func<bool> _canExecute;
        private readonly Func<object, bool> _canExecuteWithParameter;


        // =========================================================
        // Constructor 1
        // For commands WITHOUT parameters
        // =========================================================

        public RelayCommand(
            Action execute,
            Func<bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }


        // =========================================================
        // Constructor 2
        // For commands WITH parameters
        // =========================================================

        public RelayCommand(
            Action<object> execute,
            Func<object, bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _executeWithParameter = execute;
            _canExecuteWithParameter = canExecute;
        }


        // =========================================================
        // CanExecute
        // =========================================================

        public bool CanExecute(object parameter)
        {
            // Command with parameter
            if (_executeWithParameter != null)
            {
                if (_canExecuteWithParameter == null)
                    return true;

                return _canExecuteWithParameter(parameter);
            }


            // Command without parameter
            if (_canExecute == null)
                return true;

            return _canExecute();
        }


        // =========================================================
        // Execute
        // =========================================================

        public void Execute(object parameter)
        {
            // Command with parameter
            if (_executeWithParameter != null)
            {
                _executeWithParameter(parameter);
                return;
            }


            // Command without parameter
            if (_execute != null)
            {
                _execute();
            }
        }


        // =========================================================
        // CanExecuteChanged
        // =========================================================

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
    

    





