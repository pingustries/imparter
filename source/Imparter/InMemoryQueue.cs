﻿using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Imparter
{
    public class InMemoryQueue : IMessageQueue
    {
        private readonly ConcurrentQueue<IMessage> _queue;

        public InMemoryQueue()
        {
            _queue = new ConcurrentQueue<IMessage>();
        }

        public Task Enqueue(IMessage command)
        {
            _queue.Enqueue(command);
            return Task.FromResult(0);
        }

        public Task<IMessage> Dequeue()
        {
            IMessage message = null;
            _queue.TryDequeue(out message);
            return Task.FromResult(message);
        }
    }
}