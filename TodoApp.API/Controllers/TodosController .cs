using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BL;
using TodoApp.Model;

namespace TodoApp.API.Controllers
{
    
    public class TodosController : BasesController<Todo>
    {
        public TodosController(ITodoBL bl) : base(bl)
        {
        }
    }
}
