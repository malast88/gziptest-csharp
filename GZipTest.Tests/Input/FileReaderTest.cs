using GZipTest.App.Input;
using GZipTest.App.Main;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace GZipTest.Tests.Input
{
    [TestFixture]
    public class FileReaderTest
    {
        [Test]
        public void FileReaderShouldWorkAsExpected()
        {
            // Arrange
            const string fileName = "fileName";
            var uow = MockRepository.GenerateMock<IFileReaderUow>();
            var action = new Action(() => { });
            uow.Expect(t => t.ReadFileAction(fileName, JobType.Compress)).Repeat.Once().Return(action);
            var threading = MockRepository.GenerateMock<IThreading>();
            threading.Expect(t => t.ThreadStart(action)).Repeat.Once();
            var reader = new FileReader(threading, uow);

            // Act
            reader.ReadFile(fileName, JobType.Compress);

            // Assert
            threading.VerifyAllExpectations();
            uow.VerifyAllExpectations();
        }
    }
}
