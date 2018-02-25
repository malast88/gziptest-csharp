using GZipTest.App.Domain;
using GZipTest.App.Gzip;
using GZipTest.App.Process;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;

namespace GZipTest.Tests.Process
{
    [TestFixture]
    public class BlockCompressorUowFactoryTest
    {
        [Test]
        public void BlockCompressorUowFactoryShouldWorkAsExpected()
        {
            // Arrange
            var gzipStream = MockRepository.GenerateMock<IGzipStream>();
            var input = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            var output = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            var outputFactory = MockRepository.GenerateMock<IBlockCompressorUowOutputFactory>();
            outputFactory.Expect(t => t.CreateOutput()).Repeat.Once().Return(output);

            var factory = new BlockCompressorUowFactory(input, outputFactory, gzipStream);

            // Act
            factory.Create();

            // Assert
            outputFactory.VerifyAllExpectations();
        }
    }
}
