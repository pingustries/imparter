using System;
using System.Threading;
using System.Threading.Tasks;
using Imparter.Handling;
using Imparter.Store;

namespace Imparter
{
    public class SubscriberChannel
    {
        private readonly IMessageQueue _queue;
        private readonly HandlerResolver _handlers;
        private CancellationTokenSource _tokenSource;

        public SubscriberChannel(IMessageQueue queue)
        {
            _queue = queue;
            _handlers = new HandlerResolver();
        }

        public void Subscribe()
        {
            _tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => StartProcessLoop(_tokenSource.Token), _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Unsubscribe()
        {
            _tokenSource.Cancel();
        }

        public void Register<T>(Func<T, Task> handler) where T : class, IMessage
        {
            _handlers.Register(handler);
        }

        private async Task StartProcessLoop(CancellationToken tokenSourceToken)
        {
            while (!tokenSourceToken.IsCancellationRequested)
            {
                do
                {
                    IMessage message = await _queue.Dequeue();
                    if (message == null)
                        break;
                    foreach (var handler in _handlers.Resolve(message))
                    {
                        await handler(message);
                    }
                    
                } while (true);
                await Task.Delay(500, tokenSourceToken);
            }
        }
    }
}