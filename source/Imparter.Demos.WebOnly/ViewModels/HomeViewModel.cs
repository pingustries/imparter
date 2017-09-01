using System.Collections.Generic;
using Imparter.Demos.WebOnly.Todos;

namespace Imparter.Demos.WebOnly.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Todo> Todos {  get; set; }
    }
}
