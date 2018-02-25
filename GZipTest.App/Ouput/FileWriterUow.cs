using GZipTest.App.Domain;
using GZipTest.App.Io;
using GZipTest.App.Threading;
using System;
using System.Collections.Generic;

namespace GZipTest.App.Ouput
{
    public class FileWriterUow : IFileWriterUow
    {
        private readonly IEnumerable<IProducerConsumer<IByteChunk>> _inputs;
        private readonly IProducerConsumer<IByteChunk> _finishPc;
        private readonly IIo _io;

        public FileWriterUow(IIo io,
            IEnumerable<IProducerConsumer<IByteChunk>> inputs,
            IProducerConsumer<IByteChunk> finishPc)
        {
            _io = io;
            _inputs = inputs;
            _finishPc = finishPc;
        }

        public Action WriteFileAction(string fileName)
        {
            return new Action(() =>
            {
                using (var fs = _io.FileOpenWrite(fileName))
                {
                    int currId = 0;
                    while (true)
                    {
                        var allProcessorsEmpty = true;
                        foreach (var input in _inputs)
                        {
                            var task = input.Peek();
                            if (task != null)
                            {
                                allProcessorsEmpty = false;
                                if (task.Id == currId)
                                {
                                    task = input.Pop();
                                    fs.Write(task.Data, 0, task.Data.Length);
                                    currId++;
                                }
                            }
                        }
                        if (allProcessorsEmpty)
                        {
                            var finishPcData = _finishPc.Peek();
                            if (finishPcData != null && finishPcData.Id == currId)
                            {
                                break;
                            }
                        }
                    }
                }
            });
        }
    }
}
