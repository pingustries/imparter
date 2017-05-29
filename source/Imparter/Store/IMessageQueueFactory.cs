namespace Imparter.Store
{
    public interface IMessageQueueFactory
    {
        IMessageQueue Get(string name);
    }
}
