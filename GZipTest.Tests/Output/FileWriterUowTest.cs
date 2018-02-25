using GZipTest.App.Domain;
using GZipTest.App.Io;
using GZipTest.App.Ouput;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.IO;

namespace GZipTest.Tests.Output
{
    [TestFixture]
    public class FileWriterUowTest
    {
        [Test]
        public void FileWriterUowShouldWorkAsExpected()
        {
            // Arrange
            const string fileName = "fileName";

            var input1DataBytes = new byte[1];
            var input1Data = new ByteChunk { Id = 0, Data = input1DataBytes };
            var input1 = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            input1.Expect(t => t.Peek()).Repeat.Once().Return(input1Data);
            input1.Expect(t => t.Pop()).Repeat.Once().Return(input1Data);
            var input2DataBytes = new byte[1];
            var input2Data = new ByteChunk { Id = 1, Data = input2DataBytes };
            var input2 = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            input2.Expect(t => t.Peek()).Repeat.Once().Return(input2Data);
            input2.Expect(t => t.Pop()).Repeat.Once().Return(input2Data);

            var finishPc = MockRepository.GenerateMock<IProducerConsumer<IByteChunk>>();
            var finishPcData = new ByteChunk { Id = 2, Data = null };
            finishPc.Expect(t => t.Peek()).Repeat.Once().Return(finishPcData);

            var io = MockRepository.GenerateMock<IIo>();
            var outputMs = new MemoryStream();
            io.Expect(t => t.FileOpenWrite(fileName)).Repeat.Once().Return(outputMs);

            var uow = new FileWriterUow(io,
                new List<IProducerConsumer<IByteChunk>> { input1, input2 },
                finishPc);

            // Act
            uow.WriteFileAction(fileName)();

            // Assert
            input1.VerifyAllExpectations();
            input2.VerifyAllExpectations();
            io.VerifyAllExpectations();
            finishPc.VerifyAllExpectations();
        }
    }
}
