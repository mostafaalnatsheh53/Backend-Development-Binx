using Microsoft.AspNetCore.Mvc;

namespace Day4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public List<string> GetAll()
        {
            return new List<string>
            {
                "Laptop",
                "Mouse",
                "Keyboard",
                "Monitor"
            };
        }
        [HttpGet("{id}")]
        public string GetById(int id)
        {
            return $"Product ID = {id}";
        }
    }
}