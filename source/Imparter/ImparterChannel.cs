using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter
{
    public class ImparterChannel
    {
        private readonly IMessageQueue _messageQueue;

        public ImparterChannel(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public async Task Impart(object message)
        {
            await _messageQueue.Enqueue(message);
        }
    }
}
