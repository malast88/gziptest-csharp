using GZipTest.App.Domain;
using GZipTest.App.Threading;
using System.Collections.Generic;

namespace GZipTest.App.Process
{
    public interface IBlockCompressorUowOutputFactory
    {
        IProducerConsumer<IByteChunk> CreateOutput();
        IEnumerable<IProducerConsumer<IByteChunk>> GetAllOutputs();
    }
}
