using GZipTest.App.Domain;
using GZipTest.App.Input;
using GZipTest.App.Io;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using System.IO;

namespace GZipTest.Tests.Input
{
    [TestFixture]
    public class UncompressedFileReaderUowTest
    {
        [Test]
        public void UncompressedFileReaderUowShouldWorkAsExpected()
        {
            // Arrange
            const int mockBytesLength = 513;
            byte[] mockBytes = new byte[mockBytesLength];
            const int blockSize = 256;
            const string fileName = "fileName";
            var io = MockRepository.GenerateMock<IIo>();
            io.Expect(t => t.FileOpenRead(fileName)).Repeat.Once().Return(new MemoryStream(mockBytes));

            var producerConsumer = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            // with file size 513 and blockSize 256 we expect exactly 3 chunks read
            producerConsumer.Expect(t => t.Push(Arg<IByteChunk>.Matches(c => c.Id == 0 && c.Data.Length == blockSize))).Repeat.Once();
            producerConsumer.Expect(t => t.Push(Arg<IByteChunk>.Matches(c => c.Id == 1 && c.Data.Length == blockSize))).Repeat.Once();
            producerConsumer.Expect(t => t.Push(Arg<IByteChunk>.Matches(c => c.Id == 2 && c.Data.Length == mockBytesLength - 2 * blockSize))).Repeat.Once();
            producerConsumer.Expect(t => t.Stop()).Repeat.Once();

            var finishPc = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            finishPc.Expect(t => t.Push(Arg<IByteChunk>.Matches(d => d.Id == 3))).Repeat.Once();

            var readerUow = new UncompressedFileReaderUow(blockSize,
                io,
                producerConsumer,
                finishPc);

            // Act
            readerUow.ReadFileAction(fileName)();

            // Assert
            io.VerifyAllExpectations();
            producerConsumer.VerifyAllExpectations();
            finishPc.VerifyAllExpectations();
        }
    }
}
