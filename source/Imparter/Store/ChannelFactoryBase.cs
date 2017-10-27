namespace Imparter.Store
{
    public abstract class ChannelFactoryBase : IChannelFactory
    {
        private readonly IMessageTypeResolver _messageTypeResolver;
        private readonly IMessageSerializer _messageSerializer;

        public ChannelFactoryBase(IMessageTypeResolver messageTypeResolver, IMessageSerializer messageSerializer)
        {
            _messageTypeResolver = messageTypeResolver;
            _messageSerializer = messageSerializer;
        }

        protected abstract IMessageQueue Get(string name);

        public IImparterChannel GetImparterChannel(string channelName)
        {
            return new ImparterChannel(Get(channelName), _messageTypeResolver, _messageSerializer);
        }

        public ISubscriberChannel GetSubscriberChannel(string channelName)
        {
            return new PollingSubscriberChannel(Get(channelName), _messageTypeResolver, _messageSerializer);
        }
    }
}