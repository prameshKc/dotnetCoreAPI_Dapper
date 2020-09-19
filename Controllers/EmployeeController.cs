using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sample.Abstract;
using sample.Abstract.IREPO;
using sample.Models;

namespace sample.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class EmployeeController : ControllerBase
    {

        IEmployee _repo;
        public EmployeeController(IEmployee _repo)
        {
            this._repo = _repo;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEmployee()
        {
        
            return Ok(await _repo.GetAll());
        }


       [HttpPost("Save")]
        public async Task<IActionResult> SaveEmployee([FromBody] Employee emp){
        
          if(emp==null)
            return NotFound();
          try
          {
           
           await _repo.Save(emp);
            return Ok(200);
          }
          catch (System.Exception ex)
          {
              
             return BadRequest(ex);
          }
           
        }
    }
}