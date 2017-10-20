using System.Threading.Tasks;
using NLog;

namespace Imparter.Demo
{
    class TestCommandHandler
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ImparterChannel _eventChannel;

        public TestCommandHandler(ImparterChannels imparterChannels)
        {
            _eventChannel = imparterChannels.GetImparterChannel("events");
        }

        public async Task Handle(TestCommand command)
        {
            _logger.Info($"Handling command with input '{command.Input}'");
            await _eventChannel.Impart(new TestEvent { Value = $"Event because of {command.Input}" });
        }
    }
}
