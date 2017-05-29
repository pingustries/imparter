using System.Threading.Tasks;

namespace Imparter
{
    public interface IMessageQueue
    {
        Task Enqueue(IMessage command);
        Task<IMessage> Dequeue();
    }
}