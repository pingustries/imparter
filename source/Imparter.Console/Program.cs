using System;
using System.Threading.Tasks;

namespace Imparter.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandQueue = new InMemoryQueue();
            var eventQueue = new InMemoryQueue();

            var service = new ImparterTestService(commandQueue, eventQueue);
            service.Start();
            
            var sender = new MessageDispatcher(commandQueue);
            var eventHandlers = new HandlerResolver();
            eventHandlers.Register<TestEvent>(Handle);
            var eventSubscriber = new MessageSubscriber(eventQueue, eventHandlers);
            eventSubscriber.Subscribe();

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "q")
                    break;
                sender.Dispatch(new TestCommand(input)).GetAwaiter().GetResult();
            }

            eventSubscriber.Unsubscribe();
            service.Stop();
            Console.WriteLine("DONE");
        }

        private static Task Handle(TestEvent ev)
        {
            Console.WriteLine(ev.Value);
            return Task.FromResult(0);
        }
    }
}
