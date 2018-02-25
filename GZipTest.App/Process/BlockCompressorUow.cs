using GZipTest.App.Domain;
using GZipTest.App.Gzip;
using GZipTest.App.Threading;
using System;

namespace GZipTest.App.Process
{
    public class BlockCompressorUow : IBlockCompressorUow
    {
        private readonly IProducerConsumer<IByteChunk> _input;
        private readonly IProducerConsumer<IByteChunk> _output;
        private readonly IGzipStream _gzipStream;

        public BlockCompressorUow(IProducerConsumer<IByteChunk> input,
            IProducerConsumer<IByteChunk> output,
            IGzipStream gzipStream)
        {
            _input = input;
            _output = output;
            _gzipStream = gzipStream;
        }

        public Action CompressAction()
        {
            return new Action(() =>
            {
                var inputTask = _input.Pop();
                while (inputTask != null)
                {
                    var outputTask = new ByteChunk
                    {
                        Id = inputTask.Id,
                        Data = _gzipStream.Compress(inputTask.Data)
                    };
                    _output.Push(outputTask);
                    inputTask = _input.Pop();
                }
            });
        }
    }
}
