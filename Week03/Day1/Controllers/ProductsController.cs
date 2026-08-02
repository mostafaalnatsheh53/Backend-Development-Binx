using Microsoft.AspNetCore.Mvc;

namespace RestApiDesignDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static List<string> products = new()
    {
        "Laptop",
        "Mouse",
        "Keyboard"
    };

    // GET /api/products
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(products);
    }

    // GET /api/products/1
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id < 0 || id >= products.Count)
            return NotFound();

        return Ok(products[id]);
    }

    // POST /api/products
    [HttpPost]
    public IActionResult Create(string name)
    {
        products.Add(name);

        return Created("", name);
    }

    // PUT /api/products/1
    [HttpPut("{id}")]
    public IActionResult Update(int id, string name)
    {
        if (id < 0 || id >= products.Count)
            return NotFound();

        products[id] = name;

        return NoContent();
    }

    // DELETE /api/products/1
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id < 0 || id >= products.Count)
            return NotFound();

        products.RemoveAt(id);

        return NoContent();
    }
}