using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BL;
using TodoApp.Model;

namespace TodoApp.API.Controllers
{
    
    public class UsersController : BasesController<User>
    {
        public UsersController(IUserBL bl) : base(bl)
        {
        }
    }
}
