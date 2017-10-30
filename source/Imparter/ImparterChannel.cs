using System.Threading.Tasks;
using Imparter.Channels;

namespace Imparter
{
    internal class ImparterChannel : IImparterChannel
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
