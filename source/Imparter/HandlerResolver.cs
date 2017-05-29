using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imparter
{
    public class HandlerResolver<TInterface>
    {
        private readonly Dictionary<Type, Func<object, Task>> _handlers;

        public HandlerResolver()
        {
            _handlers = new Dictionary<Type, Func<object, Task>>();
        }

        public void Register<T>(Func<T, Task> handler) where T : class, TInterface
        {
            _handlers.Add(typeof(T), msg => handler(msg as T));
        }


        public async Task<Task> Dispatch(TInterface msg)
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