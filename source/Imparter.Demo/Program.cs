using System;
using Imparter.Sql;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Imparter.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            InitLogging();
            var imparterChannels = InitImparter();

            var service = new ImparterTestService(imparterChannels);
            service.Start();

            var commandChannel = imparterChannels.GetImparterChannel("commands");

            var eventChannel = imparterChannels.GetSubscriberChannel("events");
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
            Console.WriteLine("DONE");
        }

        private static ImparterChannels InitImparter()
        {
            var messageQueueFactory = new SqlServerChannelFactory(new SqlServerOptions(
                @"Data Source=(localdb)\v11.0;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"));
            messageQueueFactory.EnsureChannelExists("commands");
            messageQueueFactory.EnsureChannelExists("events");

            var imparterChannels = new ImparterChannels {ChannelFactory = messageQueueFactory};
            return imparterChannels;
        }

        private static void InitLogging()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
            LogManager.Configuration = config;
        }
    }
}
