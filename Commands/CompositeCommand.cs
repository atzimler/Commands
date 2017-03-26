using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Commands
{
    public class CompositeCommand : ICommand
    {
        private readonly List<ICommand> _commands = new List<ICommand>();

        public CompositeCommand()
        {
            
        }

        public CompositeCommand(ICommand command)
            : this(new [] { command })
        {
        }

        public CompositeCommand(IEnumerable<ICommand> commands)
        {
            _commands.AddRange(commands);

            _commands.ForEach(c => c.CanExecuteChanged += OnCanExecuteChanged);
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(sender, e);
        }

        public bool CanExecute(object parameter)
        {
            return _commands.TrueForAll(c => c.CanExecute(parameter));
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            _commands.ForEach(c => c.Execute(parameter));
        }

        public event EventHandler CanExecuteChanged;
    }
}
