using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter
{
    public class MessageImparter
    {
        private readonly IMessageQueue _messageQueue;

        public MessageImparter(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public async Task Impart(IMessage command)
        {
            await _messageQueue.Enqueue(command);
        }
    }
}
