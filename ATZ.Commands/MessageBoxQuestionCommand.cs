using ATZ.DependencyInjection;
using ATZ.DependencyInjection.System.Windows;
using JetBrains.Annotations;
using Ninject;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ATZ.Commands
{
    /// <summary>
    /// Command to ask a user a question on the View level of MVVM code structure.
    /// </summary>
    public class MessageBoxQuestionCommand : ICommand
    {
        private readonly Func<IMessageBox, MessageBoxResult?> _func;
        private bool _canExecute;

        /// <summary>
        /// The list of the MessageBoxResult values that should be considered as an Approval answer from the user.
        /// </summary>
        [NotNull]
        protected List<MessageBoxResult> Approvals { get; } = new List<MessageBoxResult>
        {
            MessageBoxResult.OK,
            MessageBoxResult.Yes
        };

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
            : this(mb => mb?.Show(messageBoxText))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(string, string)"/>
        public MessageBoxQuestionCommand(string messageBoxText, string caption)
            : this(mb => mb?.Show(messageBoxText, caption))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(string, string, MessageBoxButton)"/>
        public MessageBoxQuestionCommand(string messageBoxText, string caption, MessageBoxButton button)
            : this(mb => mb?.Show(messageBoxText, caption, button))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(string, string, MessageBoxButton, MessageBoxImage)"/>
        public MessageBoxQuestionCommand(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
            : this(mb => mb?.Show(messageBoxText, caption, button, icon))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult)"/>
        public MessageBoxQuestionCommand(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
            : this(mb => mb?.Show(messageBoxText, caption, button, icon, defaultResult))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult, MessageBoxOptions)"/>
        public MessageBoxQuestionCommand(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
            : this(mb => mb?.Show(messageBoxText, caption, button, icon, defaultResult, options))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(Window, string)"/>
        public MessageBoxQuestionCommand(Window owner, string messageBoxText)
            : this(mb => mb?.Show(owner, messageBoxText))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(Window, string, string)"/>
        public MessageBoxQuestionCommand(Window owner, string messageBoxText, string caption)
            : this(mb => mb?.Show(owner, messageBoxText, caption))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(Window, string, string, MessageBoxButton)"/>
        public MessageBoxQuestionCommand(Window owner, string messageBoxText, string caption, MessageBoxButton button)
            : this(mb => mb?.Show(owner, messageBoxText, caption, button))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(Window, string, string, MessageBoxButton, MessageBoxImage)"/>
        public MessageBoxQuestionCommand(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
            : this(mb => mb?.Show(owner, messageBoxText, caption, button, icon))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(Window, string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult)"/>
        public MessageBoxQuestionCommand(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
            : this(mb => mb?.Show(owner, messageBoxText, caption, button, icon, defaultResult))
        {
        }

        /// <summary>
        /// Creates a question that will be asked to the user.
        /// </summary>
        /// <see cref="IMessageBox.Show(Window, string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult, MessageBoxOptions)"/>
        public MessageBoxQuestionCommand(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
            : this(mb => mb?.Show(owner, messageBoxText, caption, button, icon, defaultResult, options))
        {
        }


        private MessageBoxQuestionCommand(Func<IMessageBox, MessageBoxResult?> func)
        {
            _func = func;
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
            var answer = _func(DependencyResolver.Instance.Get<IMessageBox>());
            var result = answer.HasValue && Approvals.Contains(answer.Value);
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
