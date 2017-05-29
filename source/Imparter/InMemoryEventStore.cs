using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imparter
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<StoredEvent> _events;

        public InMemoryEventStore()
        {
            _events = new List<StoredEvent>();
        }

        public Task<IEnumerable<StoredEvent>> GetEventsAfter(int currentIndex)
        { 
            return Task.FromResult(_events.Skip(currentIndex));
        }

        public Task Store(IEvent message)
        {
            _events.Add(new StoredEvent
            {
                Index = _events.Count + 1,
                Event = message
            });
            return Task.FromResult(0);
        }
    }
}
