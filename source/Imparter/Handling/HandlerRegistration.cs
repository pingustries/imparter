using System;
using System.Threading.Tasks;

namespace Imparter.Handling
{
    class HandlerRegistration
    {
        public HandlerRegistration(Type eventType, string handlerType, Func<IMessage, Task> handler)
        {
            EventType = eventType;
            HandlerType = handlerType;
            Handler = handler;
        }

        public Func<IMessage, Task> Handler { get; }

        public Type EventType { get;}
        public string HandlerType { get; }
    }
}
