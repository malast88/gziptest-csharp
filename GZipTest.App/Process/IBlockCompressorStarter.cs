using GZipTest.App.Main;

namespace GZipTest.App.Process
{
    public interface IBlockCompressorStarter
    {
        void StartCompress(JobType jobType);
    }
}
