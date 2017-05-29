using System.Threading.Tasks;

namespace Imparter.Store
{
    public interface IMessageQueue
    {
        Task Enqueue(IMessage message);
        Task<IMessage> Dequeue();
    }
}