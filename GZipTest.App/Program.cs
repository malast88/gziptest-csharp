﻿using GZipTest.App.Domain;
using GZipTest.App.Gzip;
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
        const int ProcessorProducerConsumerCapacity = 4;
        // TODO RESOLVE THIS IN RUNTIME!!!
        const int CompressThreadsCount = 4;

        static void Main(string[] args)
        {
            try
            {
                var argsResolver = new ArgumentsResolver();
                var readerToProcessorsChain = SetupReaderToProcessorsChain();
                var uncompressedFileReader = SetupUncompressedFileReader(readerToProcessorsChain);
                var blockCompressor = SetupBlockCompressor(readerToProcessorsChain);
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

        static ProducerConsumer<IByteChunk> SetupReaderToProcessorsChain()
        {
            return new ProducerConsumer<IByteChunk>(
                    ReaderProducerConsumerCapacity,
                    new ProducerConsumerQueue<IByteChunk>());
        }

        // TODO consider adding unity to the project
        static IUncompressedFileReader SetupUncompressedFileReader(
            ProducerConsumer<IByteChunk> readerToProcessorsChain)
        {
            return new UncompressedFileReader(
                new ThreadingImpl(),
                new UncompressedFileReaderUow(UncompressedReadBlockSize,
                    new IoImpl(),
                    readerToProcessorsChain));
        }

        static IBlockCompressor SetupBlockCompressor(
            ProducerConsumer<IByteChunk> readerToProcessorsChain)
        {
            var outputFactory = new BlockCompressorUowOutputFactory(ProcessorProducerConsumerCapacity);
            var blockCompressorUowFactory = new BlockCompressorUowFactory(readerToProcessorsChain, 
                outputFactory,
                new GzipStream());
            var starter = new BlockCompressorStarter(new ThreadingImpl(),
                blockCompressorUowFactory);

            return new BlockCompressor(CompressThreadsCount, starter);
        }
    }
}
