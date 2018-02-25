using System.Collections.Generic;
using GZipTest.App.Domain;
using GZipTest.App.Threading;

namespace GZipTest.App.Process
{
    public class BlockCompressorUowOutputFactory : IBlockCompressorUowOutputFactory
    {
        private int _outputCapacity;
        private List<IProducerConsumer<IByteChunk>> _data = new List<IProducerConsumer<IByteChunk>>();

        public BlockCompressorUowOutputFactory(int outputCapacity)
        {
            _outputCapacity = outputCapacity;
        }

        public IProducerConsumer<IByteChunk> CreateOutput()
        {
            var result = new ProducerConsumer<IByteChunk>(_outputCapacity,
                new ProducerConsumerOrderedQueue<IByteChunk>());
            _data.Add(result);
            return result;
        }

        public IEnumerable<IProducerConsumer<IByteChunk>> GetAllOutputs()
        {
            return _data;
        }
    }
}
