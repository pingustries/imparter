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

        public object FromTransport(string serialized, Metadata metadata)
        {
            var type = _messageTypeResolver.GetMessageType(metadata.MessageType);
            var message = _messageSerializer.Deserialize(type, serialized);
            return message;
        }

        public Metadata PrepareMetaDataForTransport(object message)
        {
            var messagetype = _messageTypeResolver.GetMessageName(message.GetType());
            var metadata = new Metadata { MessageType = messagetype };
            return metadata;
        }

        public string PrepareForTransport(object message)
        {
            var serialized = _messageSerializer.Serialize(message);
            return serialized;
        }
    }
}