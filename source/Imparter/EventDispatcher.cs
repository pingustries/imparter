using System.Threading.Tasks;

namespace Imparter
{
    public class EventDispatcher
    {
        private readonly IEventStore _store;

        public EventDispatcher(IEventStore store)
        {
            _store = store;
        }

        public async Task Dispatch(IEvent message)
        {
            await _store.Store(message);
        }
    }
}
