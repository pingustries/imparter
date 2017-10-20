using System;
using System.Threading;
using System.Threading.Tasks;
using Imparter.Handling;
using Imparter.Store;
using NLog;

namespace Imparter
{
    public class SubscriberChannel
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
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
            Task.Factory.StartNew(() => RunProcessLoop(_tokenSource.Token), _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Unsubscribe()
        {
            _logger.Debug($"Unsubscribing from {_queue.Name}");
            _tokenSource.Cancel();
        }

        public void Register<T>(Func<T, Task> handler) where T : class
        {
            _handlers.Register(handler);
        }

        private async Task RunProcessLoop(CancellationToken tokenSourceToken)
        {
            try
            {
                _logger.Debug($"Starting listening for messages on '{_queue.Name}'");
                while (!tokenSourceToken.IsCancellationRequested)
                {
                    do
                    {
                        object message = await _queue.Dequeue();
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
            catch (Exception e)
            {
                _logger.Error(e, "Exception in processing loop");
            }
        }
    }
}