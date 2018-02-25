using GZipTest.App.Gzip;
using GZipTest.App.Main;
using NUnit.Framework;
using System.Linq;

namespace GZipTest.Tests.Gzip
{
    [TestFixture]
    public class GzipStreamTest
    {
        [Test]
        public void GzipStreamShouldWorkAsExpected()
        {
            // Arrange
            var bytes = new byte[85];
            bytes[0] = 42;
            var gzip = new GzipStream();

            // Act
            var compressed = gzip.Compress(bytes, JobType.Compress);
            var uncompressed = gzip.Compress(compressed, JobType.Decompress);

            // Assert
            Assert.AreEqual(31, compressed[0]);
            Assert.AreEqual(139, compressed[1]);
            Assert.AreEqual(125, compressed.Length);
            Assert.IsTrue(bytes.AsEnumerable().SequenceEqual(uncompressed.AsEnumerable()));
        }
    }
}
