namespace Imparter.Store
{
    public interface IChannelFactory
    {
        IMessageQueue Get(string name);
    }
}
