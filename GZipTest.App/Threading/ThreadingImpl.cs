using System;
using System.Threading;

namespace GZipTest.App.Threading
{
    // no tests needed for this wrapper
    public class ThreadingImpl : IThreading
    {
        public void ThreadStart(Action threadMethod)
        {
            var th = new Thread(new ThreadStart(threadMethod));
            th.Start();
        }
    }
}
