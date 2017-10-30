using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

namespace Imparter.Demo
{
    class TestEventHandler
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public readonly Dictionary<string, object> AllResults = new Dictionary<string, object>();
        public readonly Dictionary<string, List<string>> ResultPerHandler = new Dictionary<string, List<string>>();

        public Task Handle(TestEvent arg)
        {
            var stuff = arg.Value.Split('@');
            var payload = stuff[0];
            var handler = stuff[1];
            AllResults.Add(payload, null);

            if (!ResultPerHandler.ContainsKey(handler))
            {
                ResultPerHandler.Add(handler, new List<string>());
            }
            ResultPerHandler[handler].Add(payload);
            
            _logger.Info($"Received event: '{arg.Value}'");
            return Task.FromResult(0);
        }
    }
}
