namespace GZipTest.App.Threading
{
    class ProducerConsumer<T> : IProducerConsumer<T> where T : class
    {
        public T Peek()
        {
            throw new System.NotImplementedException();
        }

        public T Pop()
        {
            throw new System.NotImplementedException();
        }

        public void Push(T task)
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
