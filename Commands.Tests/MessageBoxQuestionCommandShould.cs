using System.Windows;
using ATZ.DependencyInjection;
using ATZ.DependencyInjection.System.Windows;
using FluentAssertions;
using Moq;
using NUnit.Framework;
// ReSharper disable PossibleNullReferenceException => Moq and Fluent Assertions don't contain JetBrains annotations and as a result they are
// triggering the null reference checker. However, this is a test code, so even if there is a null reference, it is not the end of the world.
// The situation of having a null non-fatal null reference exception allows us to switch off the check on file level, because even if the
// reason for such exception is something else, it will not cause product quality issues.
// ReSharper disable AssignNullToNotNullAttribute => See explanation of PossibleNullReferenceException.

namespace Commands.Tests
{
    // TODO: Add and test various IMessageBox calls. Interface should include possibility to specify user approval values.

    [TestFixture]
    public class MessageBoxQuestionCommandShould
    {
        private Mock<IMessageBox> _messageBox;

        [SetUp]
        public void SetUp()
        {
            DependencyResolver.Initialize();

            _messageBox = new Mock<IMessageBox>();
            DependencyResolver.Instance.Bind<IMessageBox>().ToConstant(_messageBox.Object);
        }

        [Test]
        public void BeExecutableByDefault()
        {
            var cmd = new MessageBoxQuestionCommand(null);
            cmd.CanExecute(null).Should().BeTrue();
        }

        [TestCase(MessageBoxResult.OK)]
        [TestCase(MessageBoxResult.Yes)]
        public void ConsiderPositiveValuesAsApprovalByDefault(MessageBoxResult value)
        {
            _messageBox.Setup(mb => mb.Show("Question?")).Returns(value);

            var canExecuteChanged = false;
            var cmd = new MessageBoxQuestionCommand("Question?");
            cmd.CanExecuteChanged += (o, e) => { canExecuteChanged = true; };

            cmd.Execute(null);
            canExecuteChanged.Should().BeFalse();
        }

        [TestCase(MessageBoxResult.Cancel)]
        [TestCase(MessageBoxResult.No)]
        public void ConsiderNegativeValuesAsRejectionByDefault(MessageBoxResult value)
        {
            _messageBox.Setup(mb => mb.Show("Question?")).Returns(value);

            var canExecuteChanged = false;
            var cmd = new MessageBoxQuestionCommand("Question?");
            cmd.CanExecuteChanged += (o, e) => { canExecuteChanged = true; };

            cmd.Execute(null);
            canExecuteChanged.Should().BeTrue();
            cmd.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void ResetReturnsCanExecuteStateIntoTrue()
        {
            _messageBox.Setup(mb => mb.Show("Question?")).Returns(MessageBoxResult.Cancel);

            var cmd = new MessageBoxQuestionCommand("Question?");
            cmd.Execute(null);

            cmd.CanExecute(null).Should().BeFalse();

            var canExecuteChanged = false;
            cmd.CanExecuteChanged += (o, e) => { canExecuteChanged = true; };

            cmd.ResetCanExecute();
            cmd.CanExecute(null).Should().BeTrue();
            canExecuteChanged.Should().BeTrue();
        }

        [Test]
        public void ResetingCanExecuteOnExecutableCommandDoesNotRaiseCanExecuteChanged()
        {
            var cmd = new MessageBoxQuestionCommand("Question?");
            cmd.CanExecute(null).Should().BeTrue();

            var canExecuteChanged = false;
            cmd.CanExecuteChanged += (o, e) => { canExecuteChanged = true; };

            cmd.ResetCanExecute();
            cmd.CanExecute(null).Should().BeTrue();
            canExecuteChanged.Should().BeFalse();
        }
    }
}
