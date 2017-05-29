using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Imparter.Store
{
    public class InMemoryQueue : IMessageQueue
    {        
        private readonly string _name;
        private readonly ConcurrentQueue<IMessage> _queue;

        internal InMemoryQueue(string name)
        {
            _name = name;
            _queue = new ConcurrentQueue<IMessage>();
        }

        public Task Enqueue(IMessage message)
        {
            _queue.Enqueue(message);
            return Task.FromResult(0);
        }

        public Task<IMessage> Dequeue()
        {
            IMessage message = null;
            _queue.TryDequeue(out message);
            return Task.FromResult(message);
        }
    }
}