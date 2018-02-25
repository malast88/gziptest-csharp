using GZipTest.App.Domain;
using GZipTest.App.Gzip;
using GZipTest.App.Threading;

namespace GZipTest.App.Process
{
    public class BlockCompressorUowFactory : IBlockCompressorUowFactory
    {
        private readonly IProducerConsumer<IByteChunk> _input;
        private readonly IBlockCompressorUowOutputFactory _outputFactory;
        private readonly IGzipStream _gzipStream;

        public BlockCompressorUowFactory(IProducerConsumer<IByteChunk> input,
            IBlockCompressorUowOutputFactory outputFactory,
            IGzipStream gzipStream)
        {
            _input = input;
            _outputFactory = outputFactory;
            _gzipStream = gzipStream;
        }

        public IBlockCompressorUow Create()
        {
            return new BlockCompressorUow(_input, _outputFactory.CreateOutput(), _gzipStream);
        }
    }
}
