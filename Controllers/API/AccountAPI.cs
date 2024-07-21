using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.Models;

namespace Social_Media_Project.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountAPI : ControllerBase
    {
        [HttpPost]
        public IActionResult Signup([FromBody]Account pAccount)
        {
            return Ok();
        }
    }
}
