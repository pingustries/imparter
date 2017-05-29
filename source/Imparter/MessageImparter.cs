using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter
{
    public class MessageImparter
    {
        private readonly IMessageQueue _messageQueue;

        public MessageImparter(IMessageQueueFactory messageQueueFactory, string queueName)
        {
            _messageQueue = messageQueueFactory.Get(queueName);
        }

        public async Task Impart(params IMessage[] messages)
        {
            foreach (var command in messages)
            {
                await _messageQueue.Enqueue(command);
            }
        }
    }
}
