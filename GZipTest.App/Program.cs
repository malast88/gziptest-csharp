using GZipTest.App.Input;
using GZipTest.App.Main;
using GZipTest.App.Ouput;
using GZipTest.App.Process;
using System;

namespace GZipTest.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var argsResolver = new ArgumentsResolver();
                var uncompressedFileReader = new UncompressedFileReader();
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
