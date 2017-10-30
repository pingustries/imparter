namespace Imparter.Channels
{
    public interface IChannelFactory
    {
        IImparterChannel GetImparterChannel(string channelName);
        ISubscriberChannel GetSubscriberChannel(string channelName);
    }
}
