//using System.Collections.Concurrent;
//using System.Threading.Tasks;

//namespace Imparter.Transport
//{
//    public class InMemoryQueue : IMessageQueue
//    {
//        private static readonly ConcurrentDictionary<string, ConcurrentQueue<object>> Queues = new ConcurrentDictionary<string, ConcurrentQueue<object>>();
//        private readonly ConcurrentQueue<object> _queue;

//        public string Name { get; }

//        internal InMemoryQueue(string name)
//        {
//            Name = name;
//            _queue = Queues.GetOrAdd(name, n => new ConcurrentQueue<object>());
//        }

//        public Task Enqueue(object message, Metadata metadata)
//        {
//            _queue.Enqueue(message);
//            return Task.FromResult(0);
//        }

//        public Task<MessageAndMetadata> Dequeue()
//        {
//            _queue.TryDequeue(out var message);
//            return Task.FromResult(message);
//        }

//    }
//}