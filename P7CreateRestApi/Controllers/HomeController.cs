using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin")]
        public IActionResult Admin()
        {
            return Ok();
        }
    }
}