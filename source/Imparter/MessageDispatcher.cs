using System.Threading.Tasks;

namespace Imparter
{
    public class MessageDispatcher
    {
        private readonly IMessageQueue _messageQueue;

        public MessageDispatcher(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public async Task Dispatch(IMessage command)
        {
            await _messageQueue.Enqueue(command);
        }
    }
}
