using System;
using System.Windows.Input;

namespace Commands.Tests
{
    public class TestCommand : ICommand
    {
        private bool _canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void SetExecute(bool value)
        {
            _canExecute = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
