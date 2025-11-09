using AioStudy.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace AioStudy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user);
        }
    }
}
