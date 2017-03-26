using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Commands
{
    /// <summary>
    /// Command class to combine multiple ICommand implementations into one single command.
    /// </summary>
    public class CompositeCommand : ICommand
    {
        private bool _abort;

        [NotNull]
        [ItemNotNull]
        private readonly List<ICommand> _commands = new List<ICommand>();

        private object _parameter;

        /// <summary>
        /// Creates an empty CompositeCommand instance.
        /// </summary>
        public CompositeCommand()
        {
        }

        /// <summary>
        /// Creates a CompositeCommand instance that contains only one command to be executed.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public CompositeCommand(ICommand command)
            : this(new [] { command })
        {
        }

        /// <summary>
        /// Creates a CompositeCommand instance that contains a sequence of commands to be executed.
        /// </summary>
        /// <param name="commands">The commands to be executed in order.</param>
        public CompositeCommand(IEnumerable<ICommand> commands)
        {
            if (commands == null)
            {
                return;
            }

            _commands.AddRange(commands.Where(c => c != null));
            // ReSharper disable once PossibleNullReferenceException => _commands.AddRange(commands.Where(c => c != null)) => [ItemNotNull]
            _commands.ForEach(c => c.CanExecuteChanged += OnCanExecuteChanged);
        }

        private void AbortIfCanExecuteChangesToFalse(object sender, EventArgs e)
        {
            var cmd = (ICommand) sender;
            if (cmd == null)
            {
                return;
            }

            _abort = !cmd.CanExecute(_parameter);
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Returns if all of the ICommand in the sequence can execute. This means a logical and on the ICommand.CanExecute(parameter) values.
        /// </summary>
        /// <param name="parameter">The parameter of the commands.</param>
        /// <returns>True if all the commands can execute, otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            // ReSharper disable once PossibleNullReferenceException => _commands: [ItemNotNull]
            return _commands.TrueForAll(c => c.CanExecute(parameter));
        }

        /// <summary>
        /// Execute all of the commands in the sequence if CanExecute(parameter) is true. Otherwise, does nothing.
        /// </summary>
        /// <param name="parameter">The parameter to the commands.</param>
        /// <remarks>
        /// If any of the commands turns CanExecute(parameter) during the execution false, the command sequence will be aborted.
        /// It will be also aborted if the currently executed command raises a CanExecuteChanged event and querying its CanExecute(parameter)
        /// returns false.
        /// </remarks>
        public void Execute(object parameter)
        {
            _abort = false;
            _parameter = parameter;

            var commandQueue = new Queue<ICommand>();
            _commands.ForEach(commandQueue.Enqueue);
            // ReSharper disable once PossibleNullReferenceException => _commands: [ItemNotNull] => commandQueue: [ItemNotNull]
            while (commandQueue.Count > 0 && commandQueue.All(c => c.CanExecute(_parameter) && !_abort))
            {
                var cmd = commandQueue.Dequeue();
                // ReSharper disable once PossibleNullReferenceException => _commands: [ItemNotNull] => commandQueue: [ItemNotNull]
                cmd.CanExecuteChanged += AbortIfCanExecuteChangesToFalse;
                cmd.Execute(_parameter);
                cmd.CanExecuteChanged -= AbortIfCanExecuteChangesToFalse;
            }
        }

        /// <summary>
        /// Signals that the CanExecute(parameter) function return value is potentially changed. This event is raised when any of the
        /// commands in the contained sequence raises the same event.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}
