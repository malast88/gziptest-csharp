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
        [TestCase(JobType.Compress, TestName = "CoreCompress")]
        [TestCase(JobType.Decompress, TestName = "CoreDecompress")]
        public void CoreShouldWorkAsExpected(JobType jobType)
        {
            // Arrange
            var args = new string[0];
            var argResolver = MockRepository.GenerateMock<IArgumentsResolver>();
            argResolver.Expect(t => t.ResolveArgs(args)).Repeat.Once();
            argResolver.Expect(t => t.JobType).Repeat.Any().Return(jobType);
            argResolver.Expect(t => t.InputFile).Repeat.Any().Return("inputFile");
            argResolver.Expect(t => t.OutputFile).Repeat.Any().Return("outputFile");

            var fileReader = MockRepository.GenerateMock<IFileReader>();
            fileReader.Expect(t => t.ReadFile("inputFile", jobType)).Repeat.Once();

            var processor = MockRepository.GenerateMock<IBlockCompressor>();
            processor.Expect(t => t.Compress(jobType)).Repeat.Once();

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
