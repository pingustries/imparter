using System;
using System.Threading.Tasks;
using Imparter.Handling;
using Imparter.Sql;
using Imparter.Store;

namespace Imparter.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageQueueFactory = new SqlServerMessageQueueFactory(@"Data Source=(local)\sqlexpress;Initial Catalog=Imparter;Integrated Security=True");

            var service = new ImparterTestService(messageQueueFactory);
            service.Start();
            
            var commandImparter = new MessageImparter(messageQueueFactory, "commands");
            var eventHandlers = new HandlerResolver();
            eventHandlers.Register<TestEvent>(Handle);
            var eventSubscriber = new MessageSubscriber(messageQueueFactory, eventHandlers);
            eventSubscriber.Subscribe("events");

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
