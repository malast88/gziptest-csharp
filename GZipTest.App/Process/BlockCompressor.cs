using GZipTest.App.Main;

namespace GZipTest.App.Process
{
    public class BlockCompressor : IBlockCompressor
    {
        private readonly int _threadsCount;
        private readonly IBlockCompressorStarter _starter;

        public BlockCompressor(int threadsCount, IBlockCompressorStarter starter)
        {
            _threadsCount = threadsCount;
            _starter = starter;
        }

        public void Compress(JobType jobType)
        {
            for (var i=0;i<_threadsCount;i++)
            {
                _starter.StartCompress(jobType);
            }
        }
    }
}
