using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BL;
using TodoApp.Model;

namespace TodoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : BasesController<Group>
    {
        public GroupsController(IGroupBL bl) : base(bl)
        {
            
        }
    }
}
