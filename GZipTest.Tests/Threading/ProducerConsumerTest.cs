using GZipTest.App.Domain;
using GZipTest.App.Threading;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GZipTest.Tests.Threading
{
    [TestFixture]
    class ProducerConsumerTest
    {
        public class IntTask : IId
        {
            public int Id { get; set; }
            public int Data { get; set; }
        }

        [Test, Category("LongRunning")]
        // This is a HUGE test which involves actual threads and takes time, 
        // so technically it is not a unit test.
        // But this is a sort of pet-form of the whole project, and covers the main sense of it.
        // Actually, this is a code of my prototype application so it is also not well structured.
        //
        // Whats going on:
        // - "Reader" thread produces a sequence of numbers and passes to Producer-Consumer (P-C)
        // - A number of worker threads consume and "process" them (actually do nothing with a random delay)
        //      then pass to the next P-C
        // - A "writer" thread collects all the integers passed in the list.
        // The goal of the test is to ensure that the final list contains exactly the same numbers
        // as produced by "reader"
        public void ProducerConsumerShouldWork()
        {
            var readerPc = new ProducerConsumer<IntTask>(8, new ProducerConsumerQueue<IntTask>());
            var processorsPcs = new List<ProducerConsumer<IntTask>>();
            var finishPc = new ProducerConsumer<IntTask>(8, new ProducerConsumerQueue<IntTask>());

            const int integersToReadCount = 50;
            var integersToRead = new List<int>(integersToReadCount);
            var rand = new Random(DateTime.Now.Millisecond);
            for (var i=0;i< integersToReadCount;i++)
            {
                integersToRead.Add(rand.Next(10000));
            }
            var readerThread = new Thread(new ThreadStart(() =>
            {
                var rnd = new Random(DateTime.Now.Millisecond);
                var currId = 0;
                for (int i = 0; i < integersToReadCount; i++)
                {
                    var pc = new IntTask { Id = i, Data = integersToRead[i] };
                    readerPc.Push(pc);
                    Thread.Sleep(rnd.Next(10));
                    currId++;
                }
                readerPc.Stop();

                var endPc = new IntTask { Id = currId, Data = 0 };
                finishPc.Push(endPc);
            }));
            readerThread.Start();

            for (int i = 0; i < 4; i++)
            {
                var processorId = i;
                var prodCons = new ProducerConsumer<IntTask>(4, new ProducerConsumerOrderedQueue<IntTask>());
                processorsPcs.Add(prodCons);
                var processorThread = new Thread(new ThreadStart(() =>
                {
                    var rnd = new Random(DateTime.Now.Millisecond);
                    var pc = readerPc.Pop();
                    while (pc != null)
                    {
                        processorsPcs[processorId].Push(pc);
                        pc = readerPc.Pop();
                        Thread.Sleep(rnd.Next(1000));
                    }
                }));
                processorThread.Start();
            }

            var writerList = new List<int>(integersToReadCount);
            var writer = new Thread(new ThreadStart(() => {
                int currId = 0;

                while (true)
                {
                    var allProcessorsEmpty = true;
                    for (int i = 0; i < processorsPcs.Count; i++)
                    {
                        var t = processorsPcs[i].Peek();
                        if (t != null)
                        {
                            allProcessorsEmpty = false;
                            if (t.Id == currId)
                            {
                                t = processorsPcs[i].Pop();
                                writerList.Add(t.Data);
                                currId++;
                            }
                        }
                    }
                    if (allProcessorsEmpty)
                    {
                        var finishPcData = finishPc.Peek();
                        if (finishPcData != null && finishPcData.Id == currId)
                        {
                            break;
                        }
                    }

                }
            }));
            writer.Start();
            writer.Join();

            Assert.IsTrue(integersToRead.SequenceEqual(writerList));
        }
    }
}
