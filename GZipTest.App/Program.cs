using GZipTest.App.Domain;
using GZipTest.App.Input;
using GZipTest.App.Io;
using GZipTest.App.Main;
using GZipTest.App.Ouput;
using GZipTest.App.Process;
using GZipTest.App.Threading;
using System;

namespace GZipTest.App
{
    class Program
    {
        // TODO resolve this regarding to system memory and cores count available
        const int UncompressedReadBlockSize = 4 * 1024 * 1024;
        const int ReaderProducerConsumerCapacity = 8;

        static void Main(string[] args)
        {
            try
            {
                var argsResolver = new ArgumentsResolver();
                var io = new IoImpl();
                var uncompressedFileReaderToCompressorsChain = new ProducerConsumer<IByteChunk>(
                    ReaderProducerConsumerCapacity,
                    new ProducerConsumerQueue<IByteChunk>());
                var uncompressedFileReader = new UncompressedFileReader(UncompressedReadBlockSize,
                    io,
                    uncompressedFileReaderToCompressorsChain);
                var blockCompressor = new BlockCompressor();
                var fileWriter = new FileWriter();
                var core = new Core(argsResolver,
                    uncompressedFileReader,
                    blockCompressor,
                    fileWriter);
                core.Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: '{ex.Message}'");
            }
        }
    }
}
