using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GZipTest.RegressionTests
{
    [TestFixture]
    public class ExeTest
    {
        [Test, Category("Regression")]
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
            if (File.Exists(compressedFile))
            {
                File.Delete(compressedFile);
            }
            if (File.Exists(decompressedFile))
            {
                File.Delete(decompressedFile);
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $" compress \"{inputFile}\" \"{compressedFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }).WaitForExit();
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $" decompress \"{compressedFile}\" \"{decompressedFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }).WaitForExit();

            // Assert
            // check if compressed file is really GZIP file
            using (var compFs = File.OpenRead(compressedFile))
            {
                var buf = new byte[2];
                var read = compFs.Read(buf, 0, 2);
                Assert.AreEqual(2, read);
                Assert.AreEqual(0x1f, buf[0]);
                Assert.AreEqual(0x8b, buf[1]);
            }
            Assert.IsTrue(FilesAreEqual(inputFile, decompressedFile));
        }

        [Test, Category("Regression")]
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
                Assert.AreEqual("Unexpected error: 'Invalid arguments count '0''", line);
            }
        }

        [Ignore("Very resource intensive, run only on demand")]
        [TestCase(1, TestName = "TestOnGigSizeFile"), Category("SuperHugeTest")]
        public void SuperHugeTest(int sizeInGigs)
        {
            // Arrange

            var exePath = GetExePath();
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assembyDirectory = Path.GetDirectoryName(assemblyPath);
            var inputFile = Path.Combine(assembyDirectory, @"Data\huge.dat");
            var compressedFile = Path.Combine(assembyDirectory, @"Data\huge.gz");
            var decompressedFile = Path.Combine(assembyDirectory, @"Data\huge_.dat");

            GenerateHugeFile(sizeInGigs, inputFile);



            // Act
            if (File.Exists(compressedFile))
            {
                File.Delete(compressedFile);
            }
            if (File.Exists(decompressedFile))
            {
                File.Delete(decompressedFile);
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $" compress \"{inputFile}\" \"{compressedFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }).WaitForExit();
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $" decompress \"{compressedFile}\" \"{decompressedFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }).WaitForExit();

            // Assert
            // check if compressed file is really GZIP file
            using (var compFs = File.OpenRead(compressedFile))
            {
                var buf = new byte[2];
                var read = compFs.Read(buf, 0, 2);
                Assert.AreEqual(2, read);
                Assert.AreEqual(0x1f, buf[0]);
                Assert.AreEqual(0x8b, buf[1]);
            }
            Assert.IsTrue(FilesAreEqual(inputFile, decompressedFile));
        }

        private void GenerateHugeFile(int sizeInGigs, string path)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var buf = new byte[4 * 1024 * 1024];
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var inputFs = File.OpenWrite(path))
            {
                var randomAddition = random.Next(10);
                var overallLoops = sizeInGigs * 256 + randomAddition;
                for (var i = 0; i < overallLoops; i++)
                {
                    for (var j = 0; j < 1024 * 1024; j++)
                    {
                        var rnd = random.Next();
                        buf[j * 4 + 0] = (byte)rnd;
                        buf[j * 4 + 1] = (byte)(rnd >> 8);
                        buf[j * 4 + 2] = (byte)(rnd >> 0x10);
                        buf[j * 4 + 3] = (byte)(rnd >> 0x18);
                    }
                    var lengthToWrite = buf.Length;
                    if (i == overallLoops - 1)
                    {
                        lengthToWrite = Math.Max(1, random.Next(lengthToWrite));
                    }
                    inputFs.Write(buf, 0, lengthToWrite);
                }
            }
        }

        private bool FilesAreEqual(string f1, string f2)
        {
            var fi1 = new FileInfo(f1);
            var fi2 = new FileInfo(f2);
            if (fi1.Length != fi2.Length)
            {
                return false;
            }
            var buf1 = new byte[4 * 1024 * 1024];
            var buf2 = new byte[4 * 1024 * 1024];
            using (var fs1 = File.OpenRead(f1))
            {
                using (var fs2 = File.OpenRead(f2))
                {
                    var read1 = fs1.Read(buf1, 0, buf1.Length);
                    while (read1 > 0)
                    {
                        var read2 = fs2.Read(buf2, 0, buf1.Length);
                        if (read1 != read2)
                        {
                            return false;
                        }
                        for (var i=0;i<read1;i++)
                        {
                            if (buf1[i] != buf2[i])
                            {
                                return false;
                            }
                        }
                        read1 = fs1.Read(buf1, 0, buf1.Length);
                    }
                }
            }
            return true;
        }

        private string GetExePath()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assembyDirectory = Path.GetDirectoryName(assemblyPath);
            return Path.Combine(assembyDirectory, @"..\..\..\GZipTest.App\bin\Debug\GZipTest.exe");
        }
    }
}
