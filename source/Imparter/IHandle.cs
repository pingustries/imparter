using System.Threading.Tasks;

namespace Imparter
{
    public interface IHandle<in T> where T : class, IMessage
    {
        Task Handle(T message);
    }
}
