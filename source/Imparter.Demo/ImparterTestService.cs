using System;
using Imparter.Handling;
using Imparter.Store;

namespace Imparter.Demo
{
    internal class ImparterTestService
    {
        private readonly MessageSubscriber _commandSubscriber;

        public ImparterTestService(IMessageQueueFactory messageQueueFactory)
        {
            var eventImparter = new MessageImparter(messageQueueFactory, "events");

            var handlerResolver = new HandlerResolver();
            handlerResolver.Register<TestCommand>(async command => {
                Console.WriteLine($"Got {command.Input}");
                await eventImparter.Impart(new [] { new TestEvent {Value = $"Event because of {command.Input}"}});
            });
            _commandSubscriber = new MessageSubscriber(messageQueueFactory, handlerResolver);
        }

        public void Start()
        {
            _commandSubscriber.Subscribe("commands");
        }
        
        public void Stop()
        {
            _commandSubscriber.Unsubscribe();
        }
    }
}