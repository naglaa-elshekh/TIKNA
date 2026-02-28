using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                Message = "You are authorized!",
                Time = DateTime.Now
            });
        }
    }
}
