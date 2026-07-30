using Microsoft.AspNetCore.Mvc;

namespace Day4.Controllers
{
    [ApiController]
    [Route("[controller]")]
 // * Lab 2: Controller GET endpoint - Return all products

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
        // *Lab 3: Controller GET endpoint with route parameter

                public string GetById(int id)
        {
            return $"Product ID = {id}";
        }
    }
}