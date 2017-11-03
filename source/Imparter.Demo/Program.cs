using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            RunInputMode(channelFactory);
            Console.WriteLine("DONE");
        }

        private static void RunInputMode(IChannelFactory channelFactory)
        {
            var service = new ImparterTestService(channelFactory);
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
                //@"Data Source=(localdb)\v11.0;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"
            @"Data Source=(local)\SqlExpress;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"
));
            Task.WaitAll(messageQueueFactory.EnsureChannelExists("commands"), messageQueueFactory.EnsureChannelExists("events"));
            Task.WaitAll(messageQueueFactory.PurgeChannel("commands"), messageQueueFactory.PurgeChannel("events"));
            
            return messageQueueFactory;
        }

        private static void InitLogging()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = new SimpleLayout("${message} ${exception:format=message}");
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            fileTarget.Layout = new SimpleLayout("${message} ${exception:format=message}"); 
            fileTarget.FileName = new SimpleLayout("C:\\temp\\impartertest.txt");

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
            LogManager.Configuration = config;
        }
    }
}
