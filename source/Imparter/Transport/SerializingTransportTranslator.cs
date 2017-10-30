using Imparter.Store;

namespace Imparter.Transport
{
    public class SerializingTransportTranslator : ITransportTranslator
    {
        private readonly IMessageTypeResolver _messageTypeResolver;
        private readonly IMessageSerializer _messageSerializer;

        public SerializingTransportTranslator(IMessageTypeResolver messageTypeResolver, IMessageSerializer messageSerializer)
        {
            _messageTypeResolver = messageTypeResolver;
            _messageSerializer = messageSerializer;
        }

        public object FromTransport(string serializedMessage, string serializedType)
        {
            var type = _messageTypeResolver.GetMessageType(serializedType);
            var message = _messageSerializer.Deserialize(type, serializedMessage);
            return message;
        }

        public string SerialzeTypeForTransport(object message)
        {
            var messagetype = _messageTypeResolver.GetMessageName(message.GetType());
            return messagetype;
        }

        public string SerializeForTransport(object message)
        {
            var serialized = _messageSerializer.Serialize(message);
            return serialized;
        }
    }
}