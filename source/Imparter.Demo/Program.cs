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
            var messageQueueFactory = new SqlServerChannelFactory(new SqlServerSettings
            {
                ConnectionString = @"Data Source=(localdb)\v11.0;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"
            });
            messageQueueFactory.EnsureChannelExists("commands");
            messageQueueFactory.EnsureChannelExists("events");

            var imparter = new Imparter(messageQueueFactory);
            var service = new ImparterTestService(imparter);
            service.Start();


            var commandChannel = imparter.GetImparterChannel("commands");
            var eventChannel = imparter.GetSubscriberChannel("events");
            eventChannel.Register<TestEvent>(Handle);
            eventChannel.Subscribe();

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "q")
                    break;
                commandChannel.Impart(new TestCommand(input)).GetAwaiter().GetResult();
            }

            eventChannel.Unsubscribe();
            service.Stop();
            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private static Task Handle(TestEvent ev)
        {
            Console.WriteLine(ev.Value);
            return Task.FromResult(0);
        }
    }
}
