using System.Threading.Tasks;

namespace Imparter.Store
{
    public interface IMessageQueue
    {
        Task Enqueue(object message);
        Task<object> Dequeue();
        string Name { get; }
    }
}