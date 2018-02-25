using GZipTest.App.Input;
using GZipTest.App.Ouput;
using GZipTest.App.Process;

namespace GZipTest.App.Main
{
    public class Core : ICore
    {
        private readonly IArgumentsResolver _argumentsResolver;
        private readonly IFileReader _uncompressedFileReader;
        private readonly IBlockCompressor _blockCompressor;
        private readonly IFileWriter _fileWriter;

        public Core(IArgumentsResolver argumentsResolver,
            IFileReader uncompressedFileReader,
            IBlockCompressor blockCompressor,
            IFileWriter fileWriter)
        {
            _argumentsResolver = argumentsResolver;
            _uncompressedFileReader = uncompressedFileReader;
            _blockCompressor = blockCompressor;
            _fileWriter = fileWriter;
        }

        public void Run(string[] args)
        {
            _argumentsResolver.ResolveArgs(args);
            _uncompressedFileReader.ReadFile(_argumentsResolver.InputFile, _argumentsResolver.JobType);
            _blockCompressor.Compress(_argumentsResolver.JobType);
            _fileWriter.WriteFile(_argumentsResolver.OutputFile);
        }
    }
}
