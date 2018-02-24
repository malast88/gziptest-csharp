using GZipTest.App.Main;
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
                var core = new Core(argsResolver);
                core.Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: '{ex.Message}'");
            }
        }
    }
}
