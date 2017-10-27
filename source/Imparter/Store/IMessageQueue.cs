using System.Threading.Tasks;

namespace Imparter.Store
{
    public interface IMessageQueue
    {
        Task Enqueue(MessageAndMetadata message);
        Task<MessageAndMetadata> Dequeue();
        string Name { get; }
    }
}