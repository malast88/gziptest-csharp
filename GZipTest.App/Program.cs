using System;

namespace GZipTest.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var inputFile = args[0];
                var outputFile = args[1];
                System.IO.File.WriteAllText(outputFile, "test");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: '{ex.Message}'");
            }
        }
    }
}
