using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BL;
using TodoApp.Model;
using TodoApp.Model.Models.DTO;

namespace TodoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesController<T> : ControllerBase where T : IBaseModel, ICreationInfo, IModificationInfo
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] T model, int id)
        {
            try
            {
                model.SetPrimaryKey(id);
                var res = await _bl.UpdateOne(model);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var res = await _bl.DeleteOne<T>(id);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] PagingRequest pagingRequest)
        {
            if (pagingRequest == null)
            {
                return BadRequest();
            }
            try
            {
                var result = await _bl.GetPaging<T>(pagingRequest);
                return Ok(result);

            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }



    }
}
