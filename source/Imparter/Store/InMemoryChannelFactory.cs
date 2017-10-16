using System.Collections.Concurrent;

namespace Imparter.Store
{
    public class InMemoryChannelFactory : IChannelFactory
    {
        private static readonly ConcurrentDictionary<string, InMemoryQueue> Queues = new ConcurrentDictionary<string, InMemoryQueue>();

        public IMessageQueue Get(string name)
        {
            return Queues.GetOrAdd(name, queueName => new InMemoryQueue(queueName));
        }
    }
}