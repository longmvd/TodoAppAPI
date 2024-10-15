using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BL;
using TodoApp.Model;

namespace TodoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesController<T> : ControllerBase where T : IBaseModel, ICreationInfo
    {
        protected IBaseBL _bl;

        public BasesController(IBaseBL bl)
        {
            _bl = bl;
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] T model)
        {
            try
            {
                var res = await _bl.InserOne(model);
                return Ok(res);

            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
                
        }
        

        
    }
}
