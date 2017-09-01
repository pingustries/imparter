using System.Collections.Generic;

namespace Imparter.Demos.WebOnly.Todos
{
    public class TodoStore : ITodoReader
    {
        private static readonly List<Todo> Todos = new List<Todo>(new []{new Todo(1, "hei"), new Todo(2, "whaat"), });

        public void Add(string todo)
        {
            Todos.Add(new Todo(Todos.Count, todo));
        }

        public IEnumerable<Todo> GetAll()
        {
            return Todos;
        }
    }
}
