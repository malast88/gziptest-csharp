namespace GZipTest.App.Main
{
    public interface IArgumentsResolver
    {
        void ResolveArgs(string[] args);
        string InputFile { get; }
        string OutputFile { get; }
        JobType JobType { get; }
    }
}
