using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imparter.Cmd
{
    internal class ImparterTestService
    {
        private readonly MessageSubscriber _commandSubscriber;

        public ImparterTestService(InMemoryQueue commandQueue, InMemoryQueue eventQueue)
        {
            var eventDispatcher = new MessageDispatcher(eventQueue);

            var handlerResolver = new HandlerResolver();
            handlerResolver.Register<TestCommand>(async command => {
                Console.WriteLine($"Got {command.Input}");
                await eventDispatcher.Dispatch(new TestEvent {Value = $"Event because of {command.Input}"});
            });
            _commandSubscriber = new MessageSubscriber(commandQueue, handlerResolver);
            
        }

        public void Start()
        {
            _commandSubscriber.Subscribe();
        }


        public void Stop()
        {
            _commandSubscriber.Unsubscribe();
        }
    }
}