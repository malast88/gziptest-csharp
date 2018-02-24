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
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var exePath = Path.Combine(assemblyPath, @"..\..\..\..\GZipTest.App\bin\Debug\GZipTest.exe");
            var inputFile = Path.Combine(assemblyPath, @"Data\1.txt");
            var compressedFile = Path.Combine(assemblyPath, @"Data\1.gz");
            var decompressedFile = Path.Combine(assemblyPath, @"Data\1_.txt");

            // Act
            Process.Start(exePath, $"\"{inputFile}\" + \"{compressedFile}\"").WaitForExit();
            Process.Start(exePath, $"\"{compressedFile}\" + \"{decompressedFile}\"").WaitForExit();

            // Assert
            var compressedBytes = File.ReadAllBytes(compressedFile);
            Assert.AreEqual(0x1f, compressedBytes[0]);
            Assert.AreEqual(0x8b, compressedBytes[1]);
            Assert.AreEqual(File.ReadAllBytes(inputFile), File.ReadAllBytes(decompressedFile));
        }
    }
}
