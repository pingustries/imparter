using System;
using System.Threading;
using System.Threading.Tasks;
using Imparter.Handling;
using Imparter.Transport;
using NLog;

namespace Imparter
{
    internal class PollingSubscriberChannel : ISubscriberChannel
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMessageQueue _queue;
        private readonly HandlerResolver _handlers;
        private CancellationTokenSource _tokenSource;
        private Task<Task> _task;

        public PollingSubscriberChannel(IMessageQueue queue)
        {
            _queue = queue;
            _handlers = new HandlerResolver();
        }

        public void Subscribe()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                (_task ?? Task.CompletedTask).GetAwaiter().GetResult();
            }
            _tokenSource = new CancellationTokenSource();
            _task = Task.Factory.StartNew(() => RunProcessLoop(_tokenSource.Token), _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task Unsubscribe()
        {
            _logger.Debug($"Unsubscribing from {_queue.Name}");
            _tokenSource.Cancel();
            await _task;
            _task = null;
        }

        public void Register<T>(Func<T, Task> handler) where T : class
        {
            _handlers.Register(handler);
        }

        private async Task RunProcessLoop(CancellationToken tokenSourceToken)
        {
            _logger.Debug($"Starting listening for messages on '{_queue.Name}'");
            try
            {
                while (!tokenSourceToken.IsCancellationRequested)
                {
                    do
                    {
                        MessageAndMetadata messageAndMetadata = await _queue.Dequeue();
                        if (messageAndMetadata == null)
                            break;

                        try
                        {
                            foreach (var handler in _handlers.Resolve(messageAndMetadata.Message))
                            {
                                await handler(messageAndMetadata.Message);
                            }
                        }
                        catch (Exception e)
                        {
                            var metadata = GetMetadataForFailedHandling(messageAndMetadata.Metadata);
                            _logger.Warn(e, $"Failed handling message try {metadata.Tries}.");
                            await _queue.Enqueue(messageAndMetadata.Message, metadata);
                        }

                    } while (!tokenSourceToken.IsCancellationRequested);

                    try { await Task.Delay(1000, tokenSourceToken);}catch (OperationCanceledException) { }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Exception in processing loop");
            }
        }

        private Metadata GetMetadataForFailedHandling(Metadata metadata)
        {
            var tries = ++metadata.Tries;
            var shouldStop = tries > 5;
            var timeout = shouldStop ? (DateTime?)null : (DateTime.UtcNow.AddSeconds(tries * tries * 2));

            return new Metadata
            {
                Tries = tries,
                TimeoutUtc = timeout,
                IsStopped = shouldStop
            };
        }
    }
}