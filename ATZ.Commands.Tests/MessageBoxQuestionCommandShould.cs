using ATZ.DependencyInjection;
using ATZ.DependencyInjection.System.Windows;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Windows;

// ReSharper disable PossibleNullReferenceException => Moq and Fluent Assertions don't contain JetBrains annotations and as a result they are
// triggering the null reference checker. However, this is a test code, so even if there is a null reference, it is not the end of the world.
// The situation of having a null non-fatal null reference exception allows us to switch off the check on file level, because even if the
// reason for such exception is something else, it will not cause product quality issues.
// ReSharper disable AssignNullToNotNullAttribute => See explanation of PossibleNullReferenceException.

namespace ATZ.Commands.Tests
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

        [Test]
        public void ExecutingPassesCorrectParameters_S()
        {
            const string messageBoxText = "messageBoxText";

            _messageBox.Setup(m => m.Show(messageBoxText));

            var cmd = new MessageBoxQuestionCommand(messageBoxText);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        public void ExecutingPassesCorrectParameters_S_S()
        {
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";

            _messageBox.Setup(m => m.Show(messageBoxText, caption));

            var cmd = new MessageBoxQuestionCommand(messageBoxText, caption);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        public void ExecutingPassesCorrectParameters_S_S_MBB()
        {
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;

            _messageBox.Setup(m => m.Show(messageBoxText, caption, mbb));

            var cmd = new MessageBoxQuestionCommand(messageBoxText, caption, mbb);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        public void ExecutingPassesCorrectParameters_S_S_MBB_MBI()
        {
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;
            const MessageBoxImage mbi = MessageBoxImage.Error;

            _messageBox.Setup(m => m.Show(messageBoxText, caption, mbb, mbi));

            var cmd = new MessageBoxQuestionCommand(messageBoxText, caption, mbb, mbi);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        public void ExecutingPassesCorrectParameters_S_S_MBB_MBI_MBR()
        {
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;
            const MessageBoxImage mbi = MessageBoxImage.Error;
            const MessageBoxResult mbr = MessageBoxResult.No;

            _messageBox.Setup(m => m.Show(messageBoxText, caption, mbb, mbi, mbr));

            var cmd = new MessageBoxQuestionCommand(messageBoxText, caption, mbb, mbi, mbr);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        public void ExecutingPassesCorrectParameters_S_S_MBB_MBI_MBR_O()
        {
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;
            const MessageBoxImage mbi = MessageBoxImage.Error;
            const MessageBoxResult mbr = MessageBoxResult.No;
            const MessageBoxOptions o = MessageBoxOptions.DefaultDesktopOnly;

            _messageBox.Setup(m => m.Show(messageBoxText, caption, mbb, mbi, mbr, o));

            var cmd = new MessageBoxQuestionCommand(messageBoxText, caption, mbb, mbi, mbr, o);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExecutingPassesCorrectParameters_W_S()
        {
            var w = new Window();
            const string messageBoxText = "messageBoxText";

            _messageBox.Setup(m => m.Show(w, messageBoxText));

            var cmd = new MessageBoxQuestionCommand(w, messageBoxText);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExecutingPassesCorrectParameters_W_S_S()
        {
            var w = new Window();
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";

            _messageBox.Setup(m => m.Show(w, messageBoxText, caption));

            var cmd = new MessageBoxQuestionCommand(w, messageBoxText, caption);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExecutingPassesCorrectParameters_W_S_S_MBB()
        {
            var w = new Window();
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;

            _messageBox.Setup(m => m.Show(w, messageBoxText, caption, mbb));

            var cmd = new MessageBoxQuestionCommand(w, messageBoxText, caption, mbb);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExecutingPassesCorrectParameters_W_S_S_MBB_MBI()
        {
            var w = new Window();
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;
            const MessageBoxImage mbi = MessageBoxImage.Error;

            _messageBox.Setup(m => m.Show(w, messageBoxText, caption, mbb, mbi));

            var cmd = new MessageBoxQuestionCommand(w, messageBoxText, caption, mbb, mbi);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExecutingPassesCorrectParameters_W_S_S_MBB_MBI_MBR()
        {
            var w = new Window();
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;
            const MessageBoxImage mbi = MessageBoxImage.Error;
            const MessageBoxResult mbr = MessageBoxResult.No;

            _messageBox.Setup(m => m.Show(w, messageBoxText, caption, mbb, mbi, mbr));

            var cmd = new MessageBoxQuestionCommand(w, messageBoxText, caption, mbb, mbi, mbr);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ExecutingPassesCorrectParameters_W_S_S_MBB_MBI_MBR_O()
        {
            var w = new Window();
            const string messageBoxText = "messageBoxText";
            const string caption = "caption";
            const MessageBoxButton mbb = MessageBoxButton.OKCancel;
            const MessageBoxImage mbi = MessageBoxImage.Error;
            const MessageBoxResult mbr = MessageBoxResult.No;
            const MessageBoxOptions o = MessageBoxOptions.DefaultDesktopOnly;

            _messageBox.Setup(m => m.Show(w, messageBoxText, caption, mbb, mbi, mbr, o));

            var cmd = new MessageBoxQuestionCommand(w, messageBoxText, caption, mbb, mbi, mbr, o);
            cmd.Execute(null);

            _messageBox.VerifyAll();
        }

        [Test]
        public void CanUseNonDefaultApprovals()
        {
            _messageBox.Setup(mb => mb.Show("Question?")).Returns(MessageBoxResult.No);

            var cmd = new NegatedMessageBoxQuestionCommand("Question?");
            cmd.MonitorEvents();

            cmd.Execute(null);
            cmd.ShouldNotRaise(nameof(cmd.CanExecuteChanged));
        }


        // TODO: Can execute long (async).
    }
}
