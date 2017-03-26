using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ATZ.DependencyInjection;
using ATZ.DependencyInjection.System.Windows;
using JetBrains.Annotations;
using Ninject;

namespace Commands
{
    /// <summary>
    /// Command to ask a user a question on the View level of MVVM code structure.
    /// </summary>
    public class MessageBoxQuestionCommand : ICommand
    {
        [NotNull]
        private readonly List<MessageBoxResult> _approvals = new List<MessageBoxResult>
        {
            MessageBoxResult.OK,
            MessageBoxResult.Yes
        };
        private readonly string _messageBoxText;
        private bool _canExecute;

        /// <summary>
        /// Event raised when CanExecute(parameter) value is possible changed. In the current implementation, the CanExecute value is
        /// bound to user decision and as a result the parameter is ignored.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(string)"/>
        public MessageBoxQuestionCommand(string messageBoxText)
        {
            _messageBoxText = messageBoxText;
            _canExecute = true;
        }

        /// <summary>
        /// Queries if the execution of the command is allowed.
        /// </summary>
        /// <param name="parameter">Ignored, present for ICommand interface compatibility.</param>
        /// <returns>True if the command can be executed either because the user has not been asked the question yet, or because the user approved the question.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        /// <summary>
        /// Ask the user the question through a message dialog.
        /// </summary>
        /// <param name="parameter">Ignored, present for ICommand interface compatibility.</param>
        public void Execute(object parameter)
        {
            var result = _approvals.Contains(DependencyResolver.Instance.Get<IMessageBox>().Show(_messageBoxText));
            if (result == _canExecute)
            {
                return;
            }

            _canExecute = result;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reset the CanExecute flag of the command, so the question can be asked again.
        /// </summary>
        public void ResetCanExecute()
        {
            if (_canExecute)
            {
                return;
            }

            _canExecute = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
