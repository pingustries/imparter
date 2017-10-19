using Imparter.Store;

namespace Imparter
{
    public class ImparterChannels
    {
        private readonly ImparterOptions _options;

        public ImparterChannels(ImparterOptions options = null)
        {
            _options = options ?? new ImparterOptions();
        }

        public IChannelFactory ChannelFactory {
            get => _options.ChannelFactory;
            set => _options.ChannelFactory = value;
        }

        public ImparterChannel GetImparterChannel(string channelName)
        {
            return new ImparterChannel(_options.ChannelFactory.Get(channelName));
        }

        public SubscriberChannel GetSubscriberChannel(string channelName)
        {
            return new SubscriberChannel(_options.ChannelFactory.Get(channelName));
        }
    }
}
