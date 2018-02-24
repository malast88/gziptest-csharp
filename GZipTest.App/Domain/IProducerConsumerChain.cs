namespace GZipTest.App.Domain
{
    public interface IProducerConsumerChain<T> where T : class
    {
        int Count { get; }
        T Peek();
        T Pop();
        void Push(T task);
    }
}
