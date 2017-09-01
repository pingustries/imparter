using System.Collections.Generic;

namespace Imparter.Demos.WebOnly.Todos
{
    public interface ITodoReader
    {
        IEnumerable<Todo> GetAll();
    }
}