using GZipTest.App.Domain;
using System;
using System.Threading;

namespace GZipTest.App.Threading
{
    // Almost exact copy from https://ru.stackoverflow.com/questions/428327/%D0%98%D0%BC%D0%BF%D0%BB%D0%B5%D0%BC%D0%B5%D0%BD%D1%82%D0%B0%D1%86%D0%B8%D1%8F-producer-consumer-pattern
    // with minimal modifications
    public class ProducerConsumer<T> : IProducerConsumer<T> where T : class
    {
        int? _maxCapacity = null;
        object _mutex = new object();
        IProducerConsumerChain<T> _chain;
        bool _isStoppped = false;

        public ProducerConsumer(int? maxCapacity, IProducerConsumerChain<T> chain)
        {
            _maxCapacity = maxCapacity;
            _chain = chain;
        }

        public T Peek()
        {
            lock (_mutex)
            {
                if (_chain.Count == 0)
                {
                    return null;
                }

                return _chain.Peek();
            }
        }

        public T Pop()
        {
            lock (_mutex)
            {
                while (_chain.Count == 0 && !_isStoppped)
                    Monitor.Wait(_mutex);

                if (_chain.Count == 0)
                {
                    Monitor.Pulse(_mutex);
                    return null;
                }

                Monitor.Pulse(_mutex);
                return _chain.Pop();
            }
        }

        public void Push(T task)
        {
            lock (_mutex)
            {
                if (_isStoppped)
                    throw new InvalidOperationException("Queue already stopped");
                if (_maxCapacity.HasValue)
                {
                    while (_chain.Count == _maxCapacity)
                    {
                        // if we reached max capacity - release lock and wait while
                        // some consumer will consume data and Pulse
                        Monitor.Wait(_mutex);
                    }
                }
                _chain.Push(task);
                // Notify the next waiting consumer
                Monitor.Pulse(_mutex);
            }
        }

        public void Stop()
        {
            lock (_mutex)
            {
                _isStoppped = true;
                Monitor.PulseAll(_mutex);
            }
        }
    }
}
