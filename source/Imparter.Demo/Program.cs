using System;
using System.Threading.Tasks;
using Imparter.Sql;

namespace Imparter.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageQueueFactory = new SqlServerChannelFactory(new SqlServerOptions(@"Data Source=(localdb)\v11.0;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"));
            messageQueueFactory.EnsureChannelExists("commands");
            messageQueueFactory.EnsureChannelExists("events");

            var imparterChannels = new ImparterChannels{ChannelFactory = messageQueueFactory};

            var service = new ImparterTestService(imparterChannels);
            service.Start();


            var commandChannel = imparterChannels.GetImparterChannel("commands");
            var eventChannel = imparterChannels.GetSubscriberChannel("events");
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
        }

        private static Task Handle(TestEvent ev)
        {
            Console.WriteLine(ev.Value);
            return Task.FromResult(0);
        }
    }
}
