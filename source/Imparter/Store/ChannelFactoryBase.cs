namespace Imparter.Store
{
    public abstract class ChannelFactoryBase : IChannelFactory
    {
        protected abstract IMessageQueue Get(string name);

        public IImparterChannel GetImparterChannel(string channelName)
        {
            return new ImparterChannel(Get(channelName));
        }

        public ISubscriberChannel GetSubscriberChannel(string channelName)
        {
            return new PollingSubscriberChannel(Get(channelName));
        }
    }
}