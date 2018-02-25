using GZipTest.App.Gzip;
using NUnit.Framework;

namespace GZipTest.Tests.Gzip
{
    [TestFixture]
    public class GzipStreamTest
    {
        [Test]
        public void GzipStreamShouldWorkAsExpected()
        {
            // Arrange
            var bytes = new byte[1];
            var gzip = new GzipStream();

            // Act
            var result = gzip.Compress(bytes);

            // Assert
            Assert.AreEqual(0x1f, result[0]);
            Assert.AreEqual(0x8b, result[1]);
            Assert.AreEqual(121, result.Length);
        }
    }
}
