using System.Threading.Tasks;

namespace Imparter.Transport
{
    public interface IMessageQueue
    {
        Task Enqueue(object message, Metadata metadata = null);
        Task<MessageAndMetadata> Dequeue();
        string Name { get; }
    }
}