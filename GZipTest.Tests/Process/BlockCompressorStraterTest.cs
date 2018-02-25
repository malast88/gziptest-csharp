using GZipTest.App.Domain;
using GZipTest.App.Process;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace GZipTest.Tests.Process
{
    [TestFixture]
    public class BlockCompressorStraterTest
    {
        [Test]
        public void BlockCompressorStraterShouldWorkAsExpected()
        {
            // Arrange
            var compressAction = new Action(() => { });
            var uow = MockRepository.GenerateMock<IBlockCompressorUow>();
            uow.Expect(t => t.CompressAction()).Repeat.Once().Return(compressAction);
            var uowFactory = MockRepository.GenerateMock<IBlockCompressorUowFactory>();
            uowFactory.Expect(t => t.Create()).Repeat.Once().Return(uow);
            var threading = MockRepository.GenerateMock<IThreading>();
            threading.Expect(t => t.ThreadStart(compressAction)).Repeat.Once();

            var starter = new BlockCompressorStarter(threading, uowFactory);

            // Act
            starter.StartCompress();

            // Assert
            threading.VerifyAllExpectations();
            uowFactory.VerifyAllExpectations();
            uow.VerifyAllExpectations();
        }
    }
}
