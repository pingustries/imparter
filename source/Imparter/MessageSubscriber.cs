using System.Threading;
using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter
{
    public class MessageSubscriber
    {
        private readonly IMessageQueueFactory _queueFactory;
        private readonly HandlerResolver _handlers;
        private CancellationTokenSource _tokenSource;

        public MessageSubscriber(IMessageQueueFactory queueFactory, HandlerResolver handlers)
        {
            _queueFactory = queueFactory;
            _handlers = handlers;
        }

        public void Subscribe(string queueName)
        {
            _tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => StartProcessLoop(queueName, _tokenSource.Token), _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Unsubscribe()
        {
            _tokenSource.Cancel();
        }

        private async Task StartProcessLoop(string queueName, CancellationToken tokenSourceToken)
        {
            var queue = _queueFactory.Get(queueName);
            while (!tokenSourceToken.IsCancellationRequested)
            {
                do
                {
                    IMessage message = await queue.Dequeue();
                    if (message == null)
                        break;
                    await _handlers.Handle(message);
                } while (true);
                await Task.Delay(500, tokenSourceToken);
            }
        }
    }
}