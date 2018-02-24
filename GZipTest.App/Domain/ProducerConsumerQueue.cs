using System.Collections.Generic;

namespace GZipTest.App.Domain
{
    // I just do not want cover a such simple wrapper by test
    public class ProducerConsumerQueue<T> : IProducerConsumerChain<T> where T : class
    {
        private Queue<T> _queue = new Queue<T>();

        public int Count => _queue.Count;

        public T Peek()
        {
            return _queue.Peek();
        }

        public T Pop()
        {
            return _queue.Dequeue();
        }

        public void Push(T task)
        {
            _queue.Enqueue(task);
        }
    }
}
