using System;
using System.Threading;
using System.Threading.Tasks;
using Imparter.Handling;
using Imparter.Store;
using NLog;

namespace Imparter
{
    internal class PollingSubscriberChannel : ISubscriberChannel
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMessageQueue _queue;
        private readonly IMessageTypeResolver _messageTypeResolver;
        private readonly IMessageSerializer _serializer;
        private readonly HandlerResolver _handlers;
        private CancellationTokenSource _tokenSource;
        private Task<Task> _task;

        public PollingSubscriberChannel(IMessageQueue queue, IMessageTypeResolver messageTypeResolver, IMessageSerializer serializer)
        {
            _queue = queue;
            _messageTypeResolver = messageTypeResolver;
            _serializer = serializer;
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

                        var messageType = _messageTypeResolver.GetMessageType(messageAndMetadata.Metadata.MessageType);
                        var message = _serializer.Deserialize(messageType, messageAndMetadata.MessageRaw);

                        foreach (var handler in _handlers.Resolve(message))
                        {
                            await handler(message);
                        }

                    } while (!tokenSourceToken.IsCancellationRequested);
                    await Task.Delay(1000, tokenSourceToken);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Exception in processing loop");
            }
        }
    }
}