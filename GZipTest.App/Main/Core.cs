namespace GZipTest.App.Main
{
    public class Core : ICore
    {
        private readonly IArgumentsResolver _argumentsResolver;

        public Core(IArgumentsResolver argumentsResolver)
        {
            _argumentsResolver = argumentsResolver;
        }

        public void Run(string[] args)
        {
            _argumentsResolver.ResolveArgs(args);
        }
    }
}
