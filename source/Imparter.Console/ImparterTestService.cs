using System;
using System.Threading;
using System.Threading.Tasks;
using Imparter.Messages;

namespace Imparter.Cmd
{
    internal class ImparterTestService
    {
        private readonly InMemoryCommandStore _store;
        private Thread _thread;
        private volatile bool _run;
        private readonly HandlerResolver<ICommand> _handlerResolver;
        private readonly EventDispatcher _eventDispatcher;

        public ImparterTestService(InMemoryCommandStore store, InMemoryEventStore eventStore)
        {
            _store = store;
            _handlerResolver = new HandlerResolver<ICommand>();
            _eventDispatcher = new EventDispatcher(eventStore);
            _handlerResolver.Register<TestCommand>(async command => {
                Console.WriteLine($"Got {command.Input}");
                await _eventDispatcher.Dispatch(new TestEvent {Value = $"Event because of {command.Input}"});
            });
            
        }

        public void Start()
        {
            _thread = new Thread(async () => await StartProcessLoop());
            _thread.Start();
        }

        private async Task StartProcessLoop()
        {
            _run = true;
            while (_run)
            {
                await ProcessStore();
                Thread.Sleep(500);
            }
        }

        private async Task ProcessStore()
        {
            ICommand command;
            if (_store.TryDequeue(out command))
            {
                await _handlerResolver.Dispatch(command);
            }
        }

        public void Stop()
        {
            _run = false;
            _thread.Join();
        }
    }
}