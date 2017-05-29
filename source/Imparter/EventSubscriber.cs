using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imparter
{
    public class EventSubscriber
    {
        private readonly HandlerResolver<IEvent> _handlers;
        private readonly IEventStore _store;
        private CancellationTokenSource _tokenSource;

        public EventSubscriber(IEventStore store)
        {
            _store = store;
            _handlers = new HandlerResolver<IEvent>();
        }

        public void Register<T>(Action<T> func) where T : class, IEvent
        {
            _handlers.Register<T>(ev => { func(ev); return Task.FromResult(0); });
        }

        public void Subscribe(int startAt)
        {
            _tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => StartProcessLoop(0, _tokenSource.Token), _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task StartProcessLoop(int startAt, CancellationToken tokenSourceToken)
        {
            var lastProcessed = startAt;
            while (!tokenSourceToken.IsCancellationRequested)
            {
                lastProcessed = await ProcessStore(lastProcessed);
                await Task.Delay(500, tokenSourceToken);
            }
        }

        private async Task<int> ProcessStore(int lastProcessed)
        {
            var events = await _store.GetEventsAfter(lastProcessed);
            var current = lastProcessed;
            try
            {
                foreach (var storedEvent in events)
                {

                    await _handlers.Dispatch(storedEvent.Event);
                    current = storedEvent.Index;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return current;
        }

        public void Unsubscribe()
        {
            _tokenSource.Cancel();
        }
    }
}