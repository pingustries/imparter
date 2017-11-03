using System.Threading.Tasks;

namespace Imparter.Demo
{
    internal class ImparterTestService
    {
        private readonly ISubscriberChannel _incommingCommandsChannel;

        public ImparterTestService(IChannelFactory imparterChannels)
        {
            var commandHandler = new TestCommandHandler(imparterChannels);
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