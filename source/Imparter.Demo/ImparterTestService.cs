using System;

namespace Imparter.Demo
{
    internal class ImparterTestService
    {
        private readonly SubscriberChannel _incommingCommandsChannel;

        public ImparterTestService(ImparterChannels imparterChannels)
        {
            var outgoingEventsChannel = imparterChannels.GetImparterChannel("events");
            _incommingCommandsChannel = imparterChannels.GetSubscriberChannel("commands");


            _incommingCommandsChannel.Register<TestCommand>(async command => {
                Console.WriteLine($"Got {command.Input}");
                await outgoingEventsChannel.Impart(new TestEvent {Value = $"Event because of {command.Input}"});
            });
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