using GZipTest.App.Main;

namespace GZipTest.App.Input
{
    public interface IFileReader
    {
        void ReadFile(string filePath, JobType jobType);
    }
}
