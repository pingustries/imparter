using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imparter
{
    public interface IEventStore
    {
        Task<IEnumerable<StoredEvent>> GetEventsAfter(int currentIndex);
        Task Store(IEvent message);
    }
}