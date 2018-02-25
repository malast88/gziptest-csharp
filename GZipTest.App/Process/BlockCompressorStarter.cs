
using GZipTest.App.Threading;

namespace GZipTest.App.Process
{
    public class BlockCompressorStarter : IBlockCompressorStarter
    {
        private readonly IThreading _threading;
        private readonly IBlockCompressorUowFactory _factory;

        public BlockCompressorStarter(IThreading threading,
            IBlockCompressorUowFactory factory)
        {
            _threading = threading;
            _factory = factory;
        }

        public void StartCompress()
        {
            _threading.ThreadStart(_factory.Create().CompressAction());
        }
    }
}
