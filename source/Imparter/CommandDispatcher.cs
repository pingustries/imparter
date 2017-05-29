using System.Threading.Tasks;
using Imparter.Messages;

namespace Imparter
{
    public class CommandDispatcher
    {
        private readonly ICommandStore _commandStore;

        public CommandDispatcher(ICommandStore commandStore)
        {
            _commandStore = commandStore;
        }

        public async Task Dispatch(ICommand command)
        {
            await _commandStore.Store(command);
        }
    }
}
