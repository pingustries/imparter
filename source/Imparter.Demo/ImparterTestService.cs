using System.Threading.Tasks;
using Imparter.Channels;

namespace Imparter.Demo
{
    internal class ImparterTestService
    {
        private readonly ISubscriberChannel _incommingCommandsChannel;

        public ImparterTestService(IChannelFactory imparterChannels, string serviceName)
        {
            var commandHandler = new TestCommandHandler(imparterChannels, serviceName);
            _incommingCommandsChannel = imparterChannels.GetSubscriberChannel("commands");
            _incommingCommandsChannel.Register<TestCommand>(commandHandler.Handle);
        }

        public void Start()
        {
            _incommingCommandsChannel.Subscribe();
        }
        
        public Task Stop()
        {
            return _incommingCommandsChannel.Unsubscribe();
        }
    }
}