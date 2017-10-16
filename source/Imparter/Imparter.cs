using Imparter.Store;

namespace Imparter
{
    public class Imparter
    {
        private readonly IChannelFactory _channelFactory;

        public Imparter(IChannelFactory channelFactory = null)
        {
            _channelFactory = channelFactory ?? new InMemoryChannelFactory();
        }

        public ImparterChannel GetImparterChannel(string channelName)
        {
            return new ImparterChannel(_channelFactory.Get(channelName));
        }

        public SubscriberChannel GetSubscriberChannel(string channelName)
        {
            return new SubscriberChannel(_channelFactory.Get(channelName));
        }
    }
}
