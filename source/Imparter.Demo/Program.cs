using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imparter.Channels;
using Imparter.Sql;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace Imparter.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            InitLogging();
            var channelFactory = InitFactory();
            //RunInputMode(channelFactory);
            RunTestMode(channelFactory);
            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private static void RunTestMode(IChannelFactory channelFactory)
        {
            var services = new List<ImparterTestService>();
            for (int i = 0; i < 20; i++)
            {
                var service = new ImparterTestService(channelFactory, i.ToString());
                service.Start();
                services.Add(service);
            }

            var eventChannel = channelFactory.GetSubscriberChannel("events");
            var eventHandler = new TestEventHandler();
            eventChannel.Register<TestEvent>(eventHandler.Handle);
            eventChannel.Subscribe();

            var commandChannel = channelFactory.GetImparterChannel("commands");

            Console.WriteLine("All Set Up");

            for(int i = 0; i < 2000; i++)
            {
                commandChannel.Impart(new TestCommand($"{i}")).GetAwaiter().GetResult();
            }

            Console.WriteLine("all sent");
            Console.ReadKey();
            eventChannel.Unsubscribe().GetAwaiter().GetResult();


            var stops = services.Select(s => s.Stop());
            Task.WaitAll(stops.ToArray());

            Console.WriteLine($"received : {eventHandler.AllResults.Keys.Count}");
            Console.WriteLine($"\tHandler\t\tHandled");
            foreach (var handlerAndPayloads in eventHandler.ResultPerHandler)
            {
                Console.WriteLine($"\t{handlerAndPayloads.Key}\t\t{handlerAndPayloads.Value.Count}");
            }
            
        }

        private static void RunInputMode(IChannelFactory channelFactory)
        {
            var service = new ImparterTestService(channelFactory, "testService");
            service.Start();

            var commandChannel = channelFactory.GetImparterChannel("commands");

            var eventChannel = channelFactory.GetSubscriberChannel("events");
            eventChannel.Register<TestEvent>(new TestEventHandler().Handle);
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
        }

        private static IChannelFactory InitFactory()
        {
            var messageQueueFactory = new SqlServerChannelFactory(new SqlServerOptions(
                @"Data Source=(localdb)\v11.0;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"));
            messageQueueFactory.EnsureChannelExists("commands");
            messageQueueFactory.EnsureChannelExists("events");

            return messageQueueFactory;
        }

        private static void InitLogging()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = new SimpleLayout("${message}");
            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
            LogManager.Configuration = config;
        }
    }
}
