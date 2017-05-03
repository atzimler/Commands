using Moq;
using NUnit.Framework;
using System;
using System.Windows.Input;

namespace ATZ.Commands.Tests
{
    [TestFixture]
    public class IfCommandShould
    {
        [Test]
        public void ExecuteIfBranchIfConditionIsTrue()
        {
            var cmd = new Mock<ICommand>();
            cmd.Setup(c => c.Execute(null));

            var ifCommand = new IfCommand(o => true, cmd.Object);

            ifCommand.Execute(null);
            cmd.VerifyAll();
        }

        [Test]
        public void NotExecuteIfBranchIfConditionIsFalse()
        {
            var cmd = new Mock<ICommand>(MockBehavior.Strict);

            var ifCommand = new IfCommand(o => false, cmd.Object);

            ifCommand.Execute(null);
        }

        [Test]
        public void ThrowArgumentNullExceptionIfConditionIsNull()
        {
            // ReSharper disable once ObjectCreationAsStatement => We are testing the constructor.
            var ex = Assert.Throws<ArgumentNullException>(() => new IfCommand(null, null));
            Assert.IsNotNull(ex);
            Assert.AreEqual("condition", ex.ParamName);
        }

        [Test]
        public void NotCauseNullReferenceExceptionIfIfBranchIsNull()
        {
            var ifCommand = new IfCommand(o => true, null);
            Assert.DoesNotThrow(() => ifCommand.Execute(null));
        }

        [Test]
        public void ReturnTrueForCanExecuteSinceExecutionWillBeDecidedFromTheCondition()
        {
            var ifCommand = new IfCommand(o => true, null);
            Assert.IsTrue(ifCommand.CanExecute(null));
        }

        [Test]
        public void PassExecutionParameterToTheCondition()
        {
            var p = new object();
            var ifCommand = new IfCommand(o =>
            {
                Assert.AreSame(p, o);
                return true;
            }, null);
            Assert.DoesNotThrow(() => ifCommand.Execute(p));
        }

        [Test]
        public void PassParameterToTheExecutedCommand()
        {
            var p = new object();
            var cmd = new Mock<ICommand>(MockBehavior.Strict);
            cmd.Setup(c => c.Execute(p));

            var ifCommand = new IfCommand(o => true, cmd.Object);
            ifCommand.Execute(p);
        }

        [Test]
        public void ExecuteElseBranchIfConditionIsFalse()
        {
            var tcmd = new Mock<ICommand>(MockBehavior.Strict);
            var fcmd = new Mock<ICommand>(MockBehavior.Strict);
            fcmd.Setup(c => c.Execute(null));

            var ifCommand = new IfCommand(o => false, tcmd.Object, fcmd.Object);
            ifCommand.Execute(null);

            fcmd.VerifyAll();
        }

        [Test]
        public void NotThrowExceptionIfElseCommandIsNull()
        {
            var ifCommand = new IfCommand(o => false, null);
            Assert.DoesNotThrow(() => ifCommand.Execute(null));
        }

        [Test]
        public void PassParameterToTheExecutedElseBranchCommand()
        {
            var p = new object();
            var cmd = new Mock<ICommand>(MockBehavior.Strict);
            cmd.Setup(c => c.Execute(p));

            var ifCommand = new IfCommand(o => false, null, cmd.Object);
            ifCommand.Execute(p);
        }
    }
}
