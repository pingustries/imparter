using Imparter.Store;

namespace Imparter
{
    public class Imparter
    {
        private readonly IChannelFactory _channelFactory;

        public Imparter(ImparterOptions options)
        {
            _channelFactory = options.ChannelFactory;
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
