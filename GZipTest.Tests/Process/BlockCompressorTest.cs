using GZipTest.App.Main;
using GZipTest.App.Process;
using NUnit.Framework;
using Rhino.Mocks;

namespace GZipTest.Tests.Process
{
    [TestFixture]
    public class BlockCompressorTest
    {
        [Test]
        public void BlockCompressorShouldWorkAsExpected()
        {
            // Arrange
            const int threadsCount = 8;
            var strater = MockRepository.GenerateMock<IBlockCompressorStarter>();
            strater.Expect(t => t.StartCompress(JobType.Compress)).Repeat.Times(threadsCount);

            var compressor = new BlockCompressor(threadsCount, strater);

            // Act
            compressor.Compress(JobType.Compress);

            // Assert
            strater.VerifyAllExpectations();
        }
    }
}
