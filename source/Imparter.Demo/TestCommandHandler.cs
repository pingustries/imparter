using System;
using System.Threading.Tasks;
using NLog;

namespace Imparter.Demo
{
    class TestCommandHandler
    {
        private readonly string _name;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IImparterChannel _eventChannel;
        private readonly Random _random;

        public TestCommandHandler(IChannelFactory imparterChannels, string name)
        {
            _name = name;
            _eventChannel = imparterChannels.GetImparterChannel("events");
            _random = new Random();
        }

        public async Task Handle(TestCommand command)
        {
            if(_random.Next(0, 100) > 90)
                throw new Exception("Handling failed because random");
            _logger.Info($"Handling command with input '{command.Input}'");
            await _eventChannel.Impart(new TestEvent { Value = $"{command.Input}@{_name}" });
        }
    }
}
