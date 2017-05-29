using System;
using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {

            var service = new ImparterTestService();
            service.Start();
            
            var commandImparter = new MessageImparter(InMemoryQueue.Get("commands"));
            var eventHandlers = new HandlerResolver();
            eventHandlers.Register<TestEvent>(Handle);
            var eventSubscriber = new MessageSubscriber(InMemoryQueue.Get("events"), eventHandlers);
            eventSubscriber.Subscribe();

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "q")
                    break;
                commandImparter.Impart(new TestCommand(input)).GetAwaiter().GetResult();
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
