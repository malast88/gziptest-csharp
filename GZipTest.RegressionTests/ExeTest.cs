using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GZipTest.RegressionTests
{
    [TestFixture]
    public class ExeTest
    {
        [Test]
        public void ExeShouldCompressAndDecompress()
        {
            // Arrange
            var exePath = GetExePath();
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assembyDirectory = Path.GetDirectoryName(assemblyPath);
            var inputFile = Path.Combine(assembyDirectory, @"Data\1.txt");
            var compressedFile = Path.Combine(assembyDirectory, @"Data\1.gz");
            var decompressedFile = Path.Combine(assembyDirectory, @"Data\1_.txt");

            // Act
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{inputFile}\" \"{compressedFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }).WaitForExit();
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{compressedFile}\" \"{decompressedFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }).WaitForExit();

            // Assert
            var compressedBytes = File.ReadAllBytes(compressedFile);
            Assert.AreEqual(0x1f, compressedBytes[0]);
            Assert.AreEqual(0x8b, compressedBytes[1]);
            Assert.AreEqual(File.ReadAllBytes(inputFile), File.ReadAllBytes(decompressedFile));
        }

        [Test]
        public void ExeShouldCatchAnError()
        {
            // Arrange
            var exePath = GetExePath();
            
            // Act
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = string.Empty,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();

            // Assert
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                Assert.IsTrue(line.StartsWith("Unexpected error"));
            }
        }

        private string GetExePath()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assembyDirectory = Path.GetDirectoryName(assemblyPath);
            return Path.Combine(assembyDirectory, @"..\..\..\GZipTest.App\bin\Debug\GZipTest.exe");
        }
    }
}
