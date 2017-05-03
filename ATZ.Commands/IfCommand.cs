using JetBrains.Annotations;
using System;
using System.Windows.Input;

namespace ATZ.Commands
{
    /// <summary>
    /// Command to evaluate a function during execution and deciding on which branch of commands to execute, if any.
    /// </summary>
    public class IfCommand : ICommand
    {
        [NotNull]
        private readonly Func<object, bool> _condition;
        private readonly ICommand _ifBranch;
        private readonly ICommand _elseBranch;

        /// <summary>
        /// Never raised as the CanExecute always returns true, to allow evaluation of the function and decision on the commands to execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="condition">The condition to evaluate during the execution. The object passed to the condition is the parameter of the execution.</param>
        /// <param name="ifBranch">Command to execute if the condition is true. If null, no command will be executed on true condition.</param>
        /// <param name="elseBranch">Command to execute if the condition is false. If null, no command will be executed on false condition.</param>
        public IfCommand(Func<object, bool> condition, ICommand ifBranch, ICommand elseBranch = null)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _ifBranch = ifBranch;
            _elseBranch = elseBranch;
        }

        /// <summary>
        /// Always returns true, to allow evaluation of the condition during execution of the command.
        /// </summary>
        /// <param name="parameter">Ignored as the function always returns true.</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to be passed to the executed command and the condition to be evaluated.
        /// <see cref="IfCommand(Func&lt;object, bool&gt;, ICommand, ICommand)"/>
        /// </param>
        public void Execute(object parameter)
        {
            if (_condition(parameter))
            {
                _ifBranch?.Execute(parameter);
            }
            else
            {
                _elseBranch?.Execute(parameter);
            }
        }

    }
}
