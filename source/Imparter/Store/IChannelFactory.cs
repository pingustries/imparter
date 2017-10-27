namespace Imparter.Store
{
    public interface IChannelFactory
    {
        ImparterChannel GetImparterChannel(string channelName);
        SubscriberChannel GetSubscriberChannel(string channelName);
    }
}
