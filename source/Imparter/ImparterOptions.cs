using Imparter.Store;

namespace Imparter
{
    public class ImparterOptions
    {
        public ImparterOptions()
        {
            ChannelFactory = new InMemoryChannelFactory();
        }

        public IChannelFactory ChannelFactory { get; set; }
    }
}