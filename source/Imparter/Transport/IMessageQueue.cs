using System.Threading.Tasks;
using Imparter.Transport;

namespace Imparter.Channels
{
    public interface IMessageQueue
    {
        Task Enqueue(object message, Metadata metadata = null);
        Task<MessageAndMetadata> Dequeue();
        string Name { get; }
    }
}