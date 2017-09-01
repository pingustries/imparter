using System;

namespace Imparter.Demos.WebOnly.Todos
{
    public class Todo
    {
        public int Id { get; }
        public string Text { get; }
        public bool Done { get; }

        public Todo(int id, string text)
        {
            Id = id;
            Text = text;
            Done = false;
        }

        
    }
}