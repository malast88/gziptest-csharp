using System.Collections.Generic;
using System.Linq;

namespace GZipTest.App.Domain
{
    // this functionality should be covered by tests (some possible exceptions there), 
    // but I'm really lazy now
    public class ProducerConsumerOrderedQueue<T> : IProducerConsumerChain<T> where T : class, IId
    {
        private SortedDictionary<int, T> _data = new SortedDictionary<int, T>();

        public int Count => _data.Count;

        public T Peek()
        {
            var minId = _data.Keys.First();
            return _data[minId];
        }

        public T Pop()
        {
            var minId = _data.Keys.First();
            var result = _data[minId];
            _data.Remove(minId);
            return result;
        }

        public void Push(T task)
        {
            _data.Add(task.Id, task);
        }
    }
}
