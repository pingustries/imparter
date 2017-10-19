namespace Imparter.Store
{
    public class InMemoryChannelFactory : IChannelFactory
    {
        public IMessageQueue Get(string name)
        {
            return new InMemoryQueue(name);
        }
    }
}