using FluentAssertions;
using NUnit.Framework;
// ReSharper disable PossibleNullReferenceException => Moq and Fluent Assertions don't contain JetBrains annotations and as a result they are
// triggering the null reference checker. However, this is a test code, so even if there is a null reference, it is not the end of the world.
// The situation of having a null non-fatal null reference exception allows us to switch off the check on file level, because even if the
// reason for such exception is something else, it will not cause product quality issues.

namespace Commands.Tests
{
    // TODO: Add and test various IMessageBox calls. Interface should include possibility to specify user approval values.

    [TestFixture]
    public class MessageBoxQuestionCommandShould
    {
        [Test]
        public void BeExecutableByDefault()
        {
            var cmd = new MessageBoxQuestionCommand();
            cmd.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void ProperlyExecutesMessageBoxText()
        {
            
        }
    }
}
