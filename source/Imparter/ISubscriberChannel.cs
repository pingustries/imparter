using System;
using System.Threading.Tasks;

namespace Imparter
{
    public interface ISubscriberChannel
    {
        void Subscribe();
        Task Unsubscribe();
        void Register<T>(Func<T, Task> handler) where T : class;
    }
}