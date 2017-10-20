using System.Threading.Tasks;
using NLog;

namespace Imparter.Demo
{
    class TestEventHandler
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Task Handle(TestEvent arg)
        {
            _logger.Info($"Received event: '{arg.Value}'");
            return Task.FromResult(0);
        }
    }
}
