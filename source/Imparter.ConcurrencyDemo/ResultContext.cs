using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using NLog;

namespace Imparter.ConcurrencyDemo
{
    class ResultContext
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ConcurrentDictionary<int, int> Dic = new ConcurrentDictionary<int, int>();
        public static readonly Stopwatch Watch = new Stopwatch();

        public static void Start()
        {
            Watch.Start();
        }

        public static void Handled(int value, int handledBy)
        {
            Logger.Debug($"{handledBy} handled {value}");
            if (!Dic.TryAdd(value, handledBy))
            {
                string message = $"{handledBy} tried to handle {value}. But it is already handled by {Dic[value]}";
                Logger.Error(message);
                throw new Exception(message);
            }
            if (Dic.Count == Program.NumberOfMessages)
            {
                Watch.Stop();
                Console.WriteLine($"Done in {Watch.Elapsed}");
            }
        }
    }
}
