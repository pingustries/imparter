using System.Threading.Tasks;

namespace Imparter.Store
{
    public interface IMessageQueue
    {
        Task Enqueue(IMessage command);
        Task<IMessage> Dequeue();
    }
}