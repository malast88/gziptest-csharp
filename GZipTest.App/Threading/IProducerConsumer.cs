namespace GZipTest.App.Threading
{
    public interface IProducerConsumer<T> where T : class
    {
        void Push(T task);
        T Pop();
        T Peek();
        void Stop();
    }
}
