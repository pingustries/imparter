using System;
using System.Linq;
using System.Threading.Tasks;
using Imparter.Sql;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace Imparter.ConcurrencyDemo
{
    class Program
    {
        public const int NumberOfMessages = 1000;
        public const int NumberOfSubscribers = 40;
        public const int PercentageOfFailedHandlings = 10;

        static void Main(string[] args)
        {
            InitLogging();
            var logger = LogManager.GetCurrentClassLogger();
            var factory = InitFactory();

            Random random = new Random();

            var imparter = factory.GetImparterChannel("concurrencyMessages");


            for (int i = 0; i < NumberOfSubscribers; i++)
            {
                var subChannel = factory.GetSubscriberChannel("concurrencyMessages");
                var i1 = i;
                subChannel.Register<Message>(m =>
                {
                    if(random.Next(1, 100) <= PercentageOfFailedHandlings)
                        throw new Exception("Failed because of random");
                    ResultContext.Handled(m.Value, i1);
                    return Task.CompletedTask;
                });
                subChannel.Subscribe();
            }

            logger.Info($"Primed with {NumberOfSubscribers} Subscribers");

            ResultContext.Start();

            Task.WaitAll(Enumerable.Range(0, NumberOfMessages).Select(i => imparter.Impart(new Message { Value = i })).ToArray());

            logger.Info($"{NumberOfMessages} sent");

            Console.ReadKey();

        }

        private static IChannelFactory InitFactory()
        {
            var messageQueueFactory = new SqlServerChannelFactory(new SqlServerOptions(
                //@"Data Source=(localdb)\v11.0;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"
                @"Data Source=(local)\SqlExpress;Initial Catalog=ImparterTest;Integrated Security=True;Connect Timeout=30"
            ));
            Task.WaitAll(messageQueueFactory.EnsureChannelExists("concurrencyMessages"));
            Task.WaitAll(messageQueueFactory.PurgeChannel("concurrencyMessages"));

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
