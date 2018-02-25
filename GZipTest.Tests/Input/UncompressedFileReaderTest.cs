using GZipTest.App.Input;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace GZipTest.Tests.Input
{
    [TestFixture]
    public class UncompressedFileReaderTest
    {
        [Test]
        public void UncompressedFileReaderShouldWorkAsExpected()
        {
            // Arrange
            const string fileName = "fileName";
            var uow = MockRepository.GenerateMock<IUncompressedFileReaderUow>();
            var action = new Action(() => { });
            uow.Expect(t => t.ReadFileAction(fileName)).Repeat.Once().Return(action);
            var threading = MockRepository.GenerateMock<IThreading>();
            threading.Expect(t => t.ThreadStart(action)).Repeat.Once();
            var reader = new UncompressedFileReader(threading, uow);

            // Act
            reader.ReadFile(fileName);

            // Assert
            threading.VerifyAllExpectations();
            uow.VerifyAllExpectations();
        }
    }
}
