using System.Threading.Tasks;
using Imparter.Store;

namespace Imparter
{
    public class EventImparter
    {
        private readonly MessageImparter _messageImparter;

        public EventImparter(IMessageQueueFactory queueFactory, string queueName)
        {
             _messageImparter = new MessageImparter(queueFactory, queueName);
        }

        public async Task Impart(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                await _messageImparter.Impart(@event);
            }
        }
    }
}
