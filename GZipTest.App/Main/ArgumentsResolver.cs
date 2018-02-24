using System;

namespace GZipTest.App.Main
{
    public class ArgumentsResolver : IArgumentsResolver
    {
        private string _inputFile;
        private string _outputFile;

        public string InputFile => _inputFile;

        public string OutputFile => _outputFile;

        public void ResolveArgs(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException($"Invalid arguments count '{args.Length}'");
            }
            _inputFile = args[0];
            _outputFile = args[1];
        }
    }
}
