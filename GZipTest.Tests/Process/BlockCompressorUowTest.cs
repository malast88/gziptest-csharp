using GZipTest.App.Domain;
using GZipTest.App.Gzip;
using GZipTest.App.Process;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;

namespace GZipTest.Tests.Process
{
    [TestFixture]
    public class BlockCompressorUowTest
    {
        [Test]
        public void BlockCompressorUowShouldWorkAsExpected()
        {
            // Arrange
            var inputDataBytes = new byte[2];
            var inputData = new ByteChunk { Id = 123, Data = inputDataBytes };
            var input = MockRepository.GenerateStrictMock<IProducerConsumer<IByteChunk>>();
            input.Expect(t => t.Pop()).Repeat.Once().Return(inputData);
            input.Expect(t => t.Pop()).Repeat.Once().Return(null);

            var compressedBytes = new byte[1];
            var gzip = MockRepository.GenerateMock<IGzipStream>();
            gzip.Expect(t => t.Compress(inputDataBytes)).Repeat.Once().Return(compressedBytes);

            var output = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            output.Expect(t => t.Push(Arg<IByteChunk>.Matches(d => d.Id == 123 && d.Data == compressedBytes))).Repeat.Once();

            var uow = new BlockCompressorUow(input, output, gzip);

            // Act
            uow.CompressAction()();

            // Assert
            input.VerifyAllExpectations();
            output.VerifyAllExpectations();
            gzip.VerifyAllExpectations();
        }
    }
}
