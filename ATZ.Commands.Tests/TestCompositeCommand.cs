using NUnit.Framework;
using System.Windows.Input;

namespace ATZ.Commands.Tests
{
    public class TestCompositeCommand : CompositeCommand
    {
        public TestCompositeCommand(ICommand subcmd)
            : base(subcmd)
        {
        }

        public void Verify(ICommand cmd)
        {
            Assert.AreEqual(1, Commands.Count);
            Assert.AreSame(cmd, Commands[0]);
        }
    }
}
