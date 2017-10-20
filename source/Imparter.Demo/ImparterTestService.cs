namespace Imparter.Demo
{
    internal class ImparterTestService
    {
        private readonly SubscriberChannel _incommingCommandsChannel;

        public ImparterTestService(ImparterChannels imparterChannels)
        {
            var commandHandler = new TestCommandHandler(imparterChannels);
            _incommingCommandsChannel = imparterChannels.GetSubscriberChannel("commands");
            _incommingCommandsChannel.Register<TestCommand>(commandHandler.Handle);
        }

        public void Start()
        {
            _incommingCommandsChannel.Subscribe();
        }
        
        public void Stop()
        {
            _incommingCommandsChannel.Unsubscribe();
        }
    }
}