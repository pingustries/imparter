using System.Threading.Tasks;

namespace Imparter
{
    public interface IImparterChannel
    {
        Task Impart(object message);
    }
}