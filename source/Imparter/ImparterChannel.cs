using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter
{
    internal class ImparterChannel : IImparterChannel
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IMessageTypeResolver _messageTypeResolver;
        private readonly IMessageSerializer _messageSerializer;

        public ImparterChannel(IMessageQueue messageQueue, IMessageTypeResolver messageTypeResolver, IMessageSerializer messageSerializer)
        {
            _messageQueue = messageQueue;
            _messageTypeResolver = messageTypeResolver;
            _messageSerializer = messageSerializer;
        }

        public async Task Impart(object message)
        {
            var messageAndMetadata = PrepareForTransport(message);
            await _messageQueue.Enqueue(messageAndMetadata);
        }

        private MessageAndMetadata PrepareForTransport(object message)
        {
            var messagetype = _messageTypeResolver.GetMessageName(message.GetType());
            var metadata = new Metadata {MessageType = messagetype};
            var serialized = _messageSerializer.Serialize(message);
            return new MessageAndMetadata {MessageRaw = serialized, Metadata = metadata};
        }
    }
}
