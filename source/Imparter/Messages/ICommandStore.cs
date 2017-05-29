using System.Threading.Tasks;

namespace Imparter.Messages
{
    public interface ICommandStore
    {
        Task Store(ICommand command);
    }
}