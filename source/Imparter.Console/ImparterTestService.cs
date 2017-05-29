using System;
using Imparter.Store;

namespace Imparter.Cmd
{
    internal class ImparterTestService
    {
        private readonly MessageSubscriber _commandSubscriber;

        public ImparterTestService()
        {
            var eventDispatcher = new MessageImparter(new InMemoryMessageQueueFactory(), "events");

            var handlerResolver = new HandlerResolver();
            handlerResolver.Register<TestCommand>(async command => {
                Console.WriteLine($"Got {command.Input}");
                await eventDispatcher.Impart(new TestEvent {Value = $"Event because of {command.Input}"});
            });
            _commandSubscriber = new MessageSubscriber(new InMemoryMessageQueueFactory(), handlerResolver);
            
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