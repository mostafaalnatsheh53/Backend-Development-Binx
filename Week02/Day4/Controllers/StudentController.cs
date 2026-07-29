using Microsoft.AspNetCore.Mvc;

namespace Day4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Welcome Mostafa to ASP.NET Core";
        }
    }
}