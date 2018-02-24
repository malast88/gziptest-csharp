using System;

namespace GZipTest.App.Main
{
    public class ArgumentsResolver : IArgumentsResolver
    {
        private string _inputFile;
        private string _outputFile;
        private JobType _jobType;

        public string InputFile => _inputFile;

        public string OutputFile => _outputFile;

        public JobType JobType => _jobType;

        public void ResolveArgs(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException($"Invalid arguments count '{args.Length}'");
            }
            _jobType = (JobType)Enum.Parse(typeof(JobType), args[0], true);
            _inputFile = args[1];
            _outputFile = args[2];
        }
    }
}
