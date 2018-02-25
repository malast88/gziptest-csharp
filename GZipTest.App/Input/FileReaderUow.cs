using GZipTest.App.Domain;
using GZipTest.App.Io;
using GZipTest.App.Main;
using GZipTest.App.Threading;
using System;
using System.Collections.Generic;
using System.IO;

namespace GZipTest.App.Input
{
    public class FileReaderUow : IFileReaderUow
    {
        private readonly int _blockSize;
        private readonly IIo _io;
        private readonly IProducerConsumer<IByteChunk> _producerConsumer;
        private readonly IProducerConsumer<IByteChunk> _finishPc;

        public FileReaderUow(int blockSize,
            IIo io,
            IProducerConsumer<IByteChunk> producerConsumer,
            IProducerConsumer<IByteChunk> finishPc)
        {
            _blockSize = blockSize;
            _io = io;
            _producerConsumer = producerConsumer;
            _finishPc = finishPc;
        }

        public Action ReadFileAction(string filePath, JobType jobType)
        {
            if (jobType == JobType.Compress)
            {
                return () => ReadUncompressedFileAction(filePath)();
            }
            if (jobType == JobType.Decompress)
            {
                return () => ReadCompressedFileAction(filePath)();
            }
            throw new InvalidOperationException($"Unsupported job type '{jobType}'");
        }

        private Action ReadUncompressedFileAction(string filePath)
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

        private Action ReadCompressedFileAction(string filePath)
        {
            return () =>
            {

                using (var source = _io.FileOpenRead(filePath))
                {
                    int currentId = 0;

                    var reader = new FileStreamReader(source);
                    var readBytes = new MemoryStream();
                    var currByte = reader.GetNextByte();
                    if (currByte != 31)
                    {
                        throw new InvalidOperationException("not GZIP file!");
                    }
                    readBytes.WriteByte(currByte);
                    currByte = reader.GetNextByte();
                    if (currByte != 139)
                    {
                        throw new InvalidOperationException("not GZIP file!");
                    }
                    readBytes.WriteByte(currByte);

                    var gzipHeaderBytesRead = 0;
                    try
                    {
                        while (true)
                        {
                            currByte = reader.GetNextByte();
                            if (IsGzipHeaderCandidate(currByte, gzipHeaderBytesRead))
                            {
                                gzipHeaderBytesRead++;
                            }
                            else
                            {
                                if (gzipHeaderBytesRead > 0)
                                {
                                    WritePartGZipHeader(gzipHeaderBytesRead, readBytes);
                                    gzipHeaderBytesRead = 0;
                                }
                            }
                            if (IsCompleteGzipHeader(gzipHeaderBytesRead))
                            {
                                // full header read - push decompress task
                                _producerConsumer.Push(new ByteChunk
                                {
                                    Id = currentId++,
                                    Data = readBytes.ToArray()
                                });
                                readBytes = new MemoryStream();
                                WritePartGZipHeader(gzipHeaderBytesRead, readBytes);
                                gzipHeaderBytesRead = 0;
                            }
                            else
                            {
                                if (gzipHeaderBytesRead == 0)
                                {
                                    readBytes.WriteByte(currByte);
                                }
                            }
                        }
                    }
                    catch (FileEndException)
                    {
                        // just EoF - push the last decompress task
                        _producerConsumer.Push(new ByteChunk
                        {
                            Id = currentId++,
                            Data = readBytes.ToArray()
                        });
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    _producerConsumer.Stop();

                    _finishPc.Push(new ByteChunk { Id = currentId });
                }
            };
        }

        private byte[] gzipHeader = new byte[10] { 31, 139, 8, 0, 0, 0, 0, 0, 4, 0 };

        private bool IsGzipHeaderCandidate(byte b, int bytesRead)
        {
            return gzipHeader[bytesRead] == b;
        }

        private bool IsCompleteGzipHeader(int bytesRead)
        {
            return bytesRead == gzipHeader.Length;
        }

        private void WritePartGZipHeader(int bytesCount, Stream stream)
        {
            for (var i=0;i< bytesCount;i++)
            {
                stream.WriteByte(gzipHeader[i]);
            }
        }

        private class FileEndException: Exception
        {

        }

        private class FileStreamReader
        {
            private Stream _fileStream;
            private byte[] _buf = new byte[4096];
            private int _currbufIndex;
            private int _maxBufIndex;
            private bool _fileEnd = false;

            public FileStreamReader(Stream fileStream)
            {
                _fileStream = fileStream;
                ReadFromFile();
            }

            public byte GetNextByte()
            {
                if (_fileEnd)
                {
                    throw new FileEndException();
                }
                if (_currbufIndex == _maxBufIndex)
                {
                    ReadFromFile();
                }
                return _buf[_currbufIndex++];
            }

            private void ReadFromFile()
            {
                int read = _fileStream.Read(_buf, 0, _buf.Length);
                _currbufIndex = 0;
                _maxBufIndex = read;
                if (read == 0)
                {
                    _fileEnd = true;
                }
            }
        }
    }
}
