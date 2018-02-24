using GZipTest.App.Main;
using NUnit.Framework;
using System;

namespace GZipTest.Tests.Main
{
    [TestFixture]
    public class ArgumentsResolverTests
    {
        [Test]
        public void ArgumentsResolverShouldResolveArgs()
        {
            // Arrange 
            var args = new string[3] { "compress", "asd", "bsd" };
            var argResolver = new ArgumentsResolver();

            // Act
            argResolver.ResolveArgs(args);

            // Assert
            Assert.AreEqual(JobType.Compress, argResolver.JobType);
            Assert.AreEqual("asd", argResolver.InputFile);
            Assert.AreEqual("bsd", argResolver.OutputFile);
        }

        [Test]
        public void ArgumentsResolverShouldThrowErrorWhenIncorrectArgs()
        {
            // Arrange 
            var args = new string[1] { "asd"};
            var argResolver = new ArgumentsResolver();

            // Assert
            Assert.Throws(typeof(ArgumentException), new TestDelegate(() => argResolver.ResolveArgs(args)));
        }
    }
}
