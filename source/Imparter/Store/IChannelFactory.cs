namespace Imparter.Store
{
    public interface IChannelFactory
    {
        IImparterChannel GetImparterChannel(string channelName);
        ISubscriberChannel GetSubscriberChannel(string channelName);
    }
}
