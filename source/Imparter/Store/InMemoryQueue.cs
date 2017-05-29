using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Imparter.Store
{
    public class InMemoryQueue : IMessageQueue
    {
        private static readonly ConcurrentDictionary<string, InMemoryQueue> Queues = new ConcurrentDictionary<string, InMemoryQueue>();

        public static InMemoryQueue Get(string name)
        {
            return Queues.GetOrAdd(name, queueName => new InMemoryQueue(queueName));
        }
        
        private readonly string _name;
        private readonly ConcurrentQueue<IMessage> _queue;

        private InMemoryQueue(string name)
        {
            _name = name;
            _queue = new ConcurrentQueue<IMessage>();
        }

        public Task Enqueue(IMessage command)
        {
            _queue.Enqueue(command);
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