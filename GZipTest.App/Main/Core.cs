using GZipTest.App.Input;
using GZipTest.App.Ouput;
using GZipTest.App.Process;

namespace GZipTest.App.Main
{
    public class Core : ICore
    {
        private readonly IArgumentsResolver _argumentsResolver;
        private readonly IUncompressedFileReader _uncompressedFileReader;
        private readonly IBlockCompressor _blockCompressor;
        private readonly IFileWriter _fileWriter;

        public Core(IArgumentsResolver argumentsResolver,
            IUncompressedFileReader uncompressedFileReader,
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
            if (_argumentsResolver.JobType == JobType.Compress)
            {
                _uncompressedFileReader.ReadFile(_argumentsResolver.InputFile);
                _blockCompressor.Compress();
                _fileWriter.WriteFile(_argumentsResolver.OutputFile);
            }
        }
    }
}
