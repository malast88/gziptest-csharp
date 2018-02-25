using System;

namespace GZipTest.App.Threading
{
    public interface IThreading
    {
        void ThreadStart(Action threadMethod);
    }
}
