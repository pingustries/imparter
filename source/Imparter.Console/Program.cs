using System;
using Imparter.Messages;

namespace Imparter.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandStore = new InMemoryCommandStore();
            var eventStore = new InMemoryEventStore();
            var sender = new CommandDispatcher(commandStore);

            var service = new ImparterTestService(commandStore, eventStore);
            service.Start();


            var subscriber = new EventSubscriber(eventStore);
            subscriber.Register<TestEvent>(Handle);
            subscriber.Subscribe(0);


            while (true)
            {
                var input = Console.ReadLine();
                if (input == "q")
                    break;
                sender.Dispatch(new TestCommand(input)).GetAwaiter().GetResult();
            }

            subscriber.Unsubscribe();
            service.Stop();
            Console.WriteLine("DONE");
        }

        private static void Handle(TestEvent ev)
        {
            Console.WriteLine(ev.Value);
        }
    }
}
