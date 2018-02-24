using GZipTest.App.Input;
using GZipTest.App.Main;
using GZipTest.App.Ouput;
using GZipTest.App.Process;
using NUnit.Framework;
using Rhino.Mocks;
namespace GZipTest.Tests.Main
{
    [TestFixture]
    public class CoreTests
    {
        [Test]
        public void CoreShouldWorkAsExpected()
        {
            // Arrange
            var args = new string[0];
            var argResolver = MockRepository.GenerateMock<IArgumentsResolver>();
            argResolver.Expect(t => t.ResolveArgs(args)).Repeat.Once();
            argResolver.Expect(t => t.JobType).Repeat.Once().Return(JobType.Compress);
            argResolver.Expect(t => t.InputFile).Repeat.Once().Return("inputFile");
            argResolver.Expect(t => t.OutputFile).Repeat.Once().Return("outputFile");

            var fileReader = MockRepository.GenerateMock<IUncompressedFileReader>();
            fileReader.Expect(t => t.ReadFile("inputFile")).Repeat.Once();

            var processor = MockRepository.GenerateMock<IBlockCompressor>();
            processor.Expect(t => t.Compress()).Repeat.Once();

            var fileWriter = MockRepository.GenerateMock<IFileWriter>();
            fileWriter.Expect(t => t.WriteFile("outputFile")).Repeat.Once();

            var core = new Core(argResolver,
                fileReader,
                processor,
                fileWriter);

            // Act
            core.Run(args);

            // Assert
            argResolver.VerifyAllExpectations();
            fileReader.VerifyAllExpectations();
            processor.VerifyAllExpectations();
            fileWriter.VerifyAllExpectations();
        }
    }
}
