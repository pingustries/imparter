using System.Threading.Tasks;
using Imparter.Transport;

namespace Imparter.Store
{
    public interface IMessageQueue
    {
        Task Enqueue(object message);
        Task<MessageAndMetadata> Dequeue();
        string Name { get; }
    }
}