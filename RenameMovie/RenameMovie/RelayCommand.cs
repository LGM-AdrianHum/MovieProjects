// -------------------------------------------------------------------------------------------------------
// Author         :   Adrian Hum
//
// Project        :   SyncFTP/SyncFTP/RelayCommand.cs
// Create Date    :   2016-08-17  7:45 AM
// Modified       :   2016-08-17  1:11 PM
//
// -------------------------------------------------------------------------------------------------------

#region

using System;
using System.Windows.Input;

#endregion

namespace Relays
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}