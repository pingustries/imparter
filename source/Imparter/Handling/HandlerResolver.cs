using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imparter.Handling
{
    public class HandlerResolver
    {
        private readonly List<HandlerRegistration> _registeredHandlers;

        public HandlerResolver()
        {
            _registeredHandlers = new List<HandlerRegistration>();
        }

        public void Register<T>(Func<T, Task> handler) where T : class
        {
            _registeredHandlers.Add(new HandlerRegistration(typeof(T), "anonymous", msg => handler(msg as T)));
        }

        public void Register<T>(IHandle<T> handler) where T : class
        {
            _registeredHandlers.Add(new HandlerRegistration(typeof(T), handler.GetType().AssemblyQualifiedName, msg => handler.Handle(msg as T)));
        }

        public IEnumerable<Func<object, Task>> Resolve(object message)
        {
            var type = message.GetType();
            return _registeredHandlers.Where(r => r.EventType == type).Select(r => r.Handler);
        }
    }
}