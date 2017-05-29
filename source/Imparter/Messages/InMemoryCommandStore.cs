using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Imparter.Messages
{
    public class InMemoryCommandStore : ICommandStore
    {
        private readonly ConcurrentQueue<ICommand> _queue;

        public InMemoryCommandStore()
        {
            _queue = new ConcurrentQueue<ICommand>();
        }

        public Task Store(ICommand command)
        {
            _queue.Enqueue(command);
            return Task.FromResult(0);
        }

        public bool TryDequeue(out ICommand command)
        {
            return _queue.TryDequeue(out command);
        }
    }
}