using System.Windows;

namespace ATZ.Commands.Tests
{
    public class NegatedMessageBoxQuestionCommand : MessageBoxQuestionCommand
    {
        public NegatedMessageBoxQuestionCommand(string messageBoxText) : base(messageBoxText)
        {
            Approvals.Clear();
            Approvals.Add(MessageBoxResult.No);
        }
    }
}
