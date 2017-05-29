using System.Threading;
using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter
{
    public class MessageSubscriber
    {
        private readonly HandlerResolver _handlers;
        private readonly IMessageQueue _queue;
        private CancellationTokenSource _tokenSource;

        public MessageSubscriber(IMessageQueue queue, HandlerResolver handlers)
        {
            _queue = queue;
            _handlers = handlers;
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

        private async Task StartProcessLoop(CancellationToken tokenSourceToken)
        {
            while (!tokenSourceToken.IsCancellationRequested)
            {
                bool somethingProcessed;
                do
                {
                    somethingProcessed = await ProcessStore();
                } while (somethingProcessed);
                await Task.Delay(500, tokenSourceToken);
            }
        }

        private async Task<bool> ProcessStore()
        {
            IMessage message = await _queue.Dequeue();
            if (message == null)
                return false;
            await _handlers.Handle(message);
            return true;
        }
    }
}