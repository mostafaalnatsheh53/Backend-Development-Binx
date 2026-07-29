using Microsoft.AspNetCore.Mvc;

namespace Day4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
return "My name is Mostafa";        }
    }
}