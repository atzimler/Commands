using System;
using System.Windows.Input;

namespace Commands
{
    /// <summary>
    /// Command to ask a user a question on the View level of MVVM code structure.
    /// </summary>
    public class MessageBoxQuestionCommand : ICommand
    {
        /// <summary>
        /// Event raised when CanExecute(parameter) value is possible changed. In the current implementation, the CanExecute value is
        /// bound to user decision and as a result the parameter is ignored.
        /// </summary>
        public event EventHandler CanExecuteChanged;


        /// <summary>
        /// Queries if the execution of the command is allowed.
        /// </summary>
        /// <param name="parameter">Ignored, present for ICommand interface compatibility.</param>
        /// <returns>True if the command can be executed either because the user has not been asked the question yet, or because the user approved the question.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Ask the user the question through a message dialog.
        /// </summary>
        /// <param name="parameter">Ignored, present for ICommand interface compatibility.</param>
        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reset the CanExecute flag of the command, so the question can be asked again.
        /// </summary>
        public void ResetCanExecute()
        {
            
        }
    }
}
