using GZipTest.App.Domain;
using GZipTest.App.Io;
using GZipTest.App.Threading;
using System;

namespace GZipTest.App.Input
{
    public class UncompressedFileReaderUow : IUncompressedFileReaderUow
    {
        private readonly int _blockSize;
        private readonly IIo _io;
        private readonly IProducerConsumer<IByteChunk> _producerConsumer;
        private readonly IProducerConsumer<IByteChunk> _finishPc;

        public UncompressedFileReaderUow(int blockSize,
            IIo io,
            IProducerConsumer<IByteChunk> producerConsumer,
            IProducerConsumer<IByteChunk> finishPc)
        {
            _blockSize = blockSize;
            _io = io;
            _producerConsumer = producerConsumer;
            _finishPc = finishPc;
        }

        public Action ReadFileAction(string filePath)
        {
            return () =>
            {
                using (var source = _io.FileOpenRead(filePath))
                {
                    int currentId = 0;
                    byte[] buffer = new byte[_blockSize];
                    int bytesRead;
                    while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        var bytes = buffer;
                        if (bytesRead != buffer.Length)
                        {
                            bytes = new byte[bytesRead];
                            Buffer.BlockCopy(buffer, 0, bytes, 0, bytesRead);
                        }
                        var byteChunk = new ByteChunk
                        {
                            Id = currentId++,
                            Data = bytes
                        };
                        _producerConsumer.Push(byteChunk);

                        if (bytesRead > 0)
                        {
                            buffer = new byte[_blockSize];
                        }
                    }
                    _producerConsumer.Stop();

                    _finishPc.Push(new ByteChunk { Id = currentId });
                }
            };
        }
    }
}
