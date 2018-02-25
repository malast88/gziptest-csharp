using GZipTest.App.Main;

namespace GZipTest.App.Process
{
    public interface IBlockCompressor
    {
        void Compress(JobType jobType);
    }
}
