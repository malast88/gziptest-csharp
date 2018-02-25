using GZipTest.App.Ouput;
using GZipTest.App.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace GZipTest.Tests.Output
{
    [TestFixture]
    public class FileWriterTest
    {
        [Test]
        public void FileWriteShouldWorkAsExpected()
        {
            // Arrange
            const string fileName = "fileName";
            var uow = MockRepository.GenerateMock<IFileWriterUow>();
            var action = new Action(() => { });
            uow.Expect(t => t.WriteFileAction(fileName)).Repeat.Once().Return(action);
            var threading = MockRepository.GenerateMock<IThreading>();
            threading.Expect(t => t.ThreadStart(action)).Repeat.Once();
            var reader = new FileWriter(threading, uow);

            // Act
            reader.WriteFile(fileName);

            // Assert
            threading.VerifyAllExpectations();
            uow.VerifyAllExpectations();
        }
    }
}
