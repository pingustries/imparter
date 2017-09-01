using Imparter.Demos.WebOnly.Todos;
using Imparter.Demos.WebOnly.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Imparter.Demos.WebOnly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITodoReader _todoStore;

        public HomeController()
        {
            _todoStore = new TodoStore();
            _messageImparter = new MessageImparter
        }

        [HttpGet]
        public IActionResult Index()
        {
            var todos = _todoStore.GetAll();
            var vm = new HomeViewModel
            {
                Todos = todos
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Add(string text)
        {
            return null;
        }
    }
}
