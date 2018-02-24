using GZipTest.App.Main;
using NUnit.Framework;
using Rhino.Mocks;
namespace GZipTest.Tests.Main
{
    [TestFixture]
    public class CoreTests
    {
        [Test]
        public void CoreShouldWorkAsExpected()
        {
            // Arrange
            var args = new string[0];
            var argResolver = MockRepository.GenerateMock<IArgumentsResolver>();
            argResolver.Expect(t => t.ResolveArgs(args)).Repeat.Once();
            var core = new Core(argResolver);

            // Act
            core.Run(args);

            // Assert
            argResolver.VerifyAllExpectations();
        }
    }
}
