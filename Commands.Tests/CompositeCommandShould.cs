using System.Windows.Input;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Commands.Tests
{
    [TestFixture]
    public class CompositeCommandShould
    {
        [Test]
        public void BeAbleToExecuteEmptyCommand()
        {
            var cmd = new CompositeCommand();
            cmd.CanExecute(null).Should().BeTrue();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ReturnCanExecuteOfContainedCommandIfThereIsOnlyOneCommand(bool canExecuteSubCommand)
        {
            var subcmd = new Mock<ICommand>();
            subcmd.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(canExecuteSubCommand);

            var cmd = new CompositeCommand(subcmd.Object);
            cmd.CanExecute(null).Should().Be(canExecuteSubCommand);
        }

        [TestCase(false, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, true)]
        public void ReturnCombinedCanExecuteCommandIfThereIsMoreThanOneCommand(
            bool canExecuteSubCommand1, bool canExecuteSubCommand2, bool canExecuteCompositeCommand)
        {
            var subcmd1 = new Mock<ICommand>();
            subcmd1.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(canExecuteSubCommand1);

            var subcmd2 = new Mock<ICommand>();
            subcmd2.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(canExecuteSubCommand2);

            var cmd = new CompositeCommand(new[] {subcmd1.Object, subcmd2.Object});
            cmd.CanExecute(null).Should().Be(canExecuteCompositeCommand);
        }

        [Test]
        public void ExecuteEmptyCommandWithoutAnyProblem()
        {
            var cmd = new CompositeCommand();
            Assert.DoesNotThrow(() => cmd.Execute(null));
        }

        // TODO: Should handle can execute change.

        [Test]
        public void ExecuteOneSubCommand()
        {
            var subcmd = new Mock<ICommand>();
            subcmd.Setup(c => c.CanExecute(null)).Returns(true);

            var cmd = new CompositeCommand(subcmd.Object);
            cmd.Execute(null);

            subcmd.Verify(c => c.Execute(null));
        }

        [Test]
        public void ExecuteMultipleSubCommands()
        {
            var subcmd1 = new Mock<ICommand>();
            subcmd1.Setup(c => c.CanExecute(null)).Returns(true);

            var subcmd2 = new Mock<ICommand>();
            subcmd2.Setup(c => c.CanExecute(null)).Returns(true);

            var cmd = new CompositeCommand(new[] {subcmd1.Object, subcmd2.Object});
            cmd.Execute(null);

            subcmd1.Verify(c => c.Execute(null));
            subcmd2.Verify(c => c.Execute(null));
        }

        [Test]
        public void CheckEachOfTheSubCommandsForCanExecuteBeforeExecuting()
        {
            var subcmd1 = new Mock<ICommand>();
            subcmd1.Setup(c => c.CanExecute(null)).Returns(true);

            var subcmd2 = new Mock<ICommand>();
            subcmd2.Setup(c => c.CanExecute(null)).Returns(true);

            var cmd = new CompositeCommand(new [] {subcmd1.Object, subcmd2.Object});
            cmd.Execute(null);

            subcmd1.Verify(c => c.CanExecute(null));
            subcmd2.Verify(c => c.CanExecute(null));
        }

        [Test]
        public void CheckEnoughSubCommandsToKnowThatItCanExecuteOrNotBeforeExecuting()
        {
            var subcmd1 = new Mock<ICommand>();
            subcmd1.Setup(c => c.CanExecute(null)).Returns(false);

            var subcmd2 = new Mock<ICommand>(MockBehavior.Strict);

            var cmd = new CompositeCommand(new [] {subcmd1.Object, subcmd2.Object});
            cmd.Execute(null);

            subcmd1.Verify(c => c.CanExecute(null));
        }

        [Test]
        public void NotExecuteCommandWhenCanExecuteIsFalse()
        {
            var subcmd = new Mock<ICommand>(MockBehavior.Strict);
            subcmd.Setup(c => c.CanExecute(null)).Returns(false);

            var cmd = new CompositeCommand(subcmd.Object);
            cmd.Execute(null);
        }

        [Test]
        public void PassCorrectParameterForCanExecutes()
        {
            var parameter = "parameter";

            var subcmd = new Mock<ICommand>(MockBehavior.Strict);
            subcmd.Setup(c => c.CanExecute(parameter)).Returns(false);

            var cmd = new CompositeCommand(subcmd.Object);
            cmd.CanExecute(parameter);
            
            subcmd.Verify(c => c.CanExecute(parameter));
        }

        [Test]
        public void PassCorrectParameterForExecute()
        {
            var parameter = "parameter";

            var subcmd = new Mock<ICommand>(MockBehavior.Strict);
            subcmd.Setup(c => c.CanExecute(parameter)).Returns(true);
            subcmd.Setup(c => c.Execute(parameter));

            var cmd = new CompositeCommand(subcmd.Object);
            cmd.Execute(parameter);

            subcmd.VerifyAll();
        }

        [Test]
        public void ExecuteTheCommandsInCorrectOrder()
        {
            var subcmd1 = new Mock<ICommand>();
            subcmd1.Setup(c => c.CanExecute(null)).Returns(true);

            var subcmd2 = new Mock<ICommand>();
            subcmd2.Setup(c => c.CanExecute(null)).Returns(true);

            var cmd = new CompositeCommand(new[] {subcmd1.Object, subcmd2.Object});

            var call = 0;
            subcmd1.Setup(c => c.Execute(null)).Callback(() => call++.Should().Be(0));
            subcmd2.Setup(c => c.Execute(null)).Callback(() => call++.Should().Be(1));

            cmd.Execute(null);
        }

        [Test]
        public void NotExecuteAnyCommandIfCanExecuteIsFalse()
        {
            var subcmd1 = new Mock<ICommand>(MockBehavior.Strict);
            subcmd1.Setup(c => c.CanExecute(null)).Returns(true);

            var subcmd2 = new Mock<ICommand>(MockBehavior.Strict);
            subcmd2.Setup(c => c.CanExecute(null)).Returns(false);

            var cmd = new CompositeCommand(new [] {subcmd1.Object, subcmd2.Object});
            cmd.Execute(null);
        }


    }
}
