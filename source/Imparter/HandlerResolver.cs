using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imparter
{
    public class HandlerResolver
    {
        private readonly Dictionary<Type, Func<object, Task>> _handlers;

        public HandlerResolver()
        {
            _handlers = new Dictionary<Type, Func<object, Task>>();
        }

        public void Register<T>(Func<T, Task> handler) where T : class, IMessage
        {
            _handlers.Add(typeof(T), msg => handler(msg as T));
        }

        public void Register<T>(IHandle<T> handler) where T : class, IMessage
        {
            _handlers.Add(typeof(T), msg => handler.Handle(msg as T));
        }

        public async Task<Task> Handle(IMessage msg)
        {
            Func<object, Task> handler;
            if (_handlers.TryGetValue(msg.GetType(), out handler))
            {
                await handler(msg);
            }
            return Task.FromResult(0);
        }
    }
}