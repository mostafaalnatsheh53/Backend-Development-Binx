// Hands-On Lab: Design a REST Resource Map

// Primary Resource: Books
using Microsoft.AspNetCore.Mvc;

namespace Day1.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BooksController : ControllerBase
{
    private static List<string> books = new()
    {
        "Clean Code",
        "The Pragmatic Programmer",
        "Design Patterns"
    };

    // GET: api/v1/books
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(books);
    }

    // GET: api/v1/books/1
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id < 0 || id >= books.Count)
            return NotFound("Book not found.");

        return Ok(books[id]);
    }

    // POST: api/v1/books?title=Refactoring
    [HttpPost]
    public IActionResult Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Book title is required.");

        books.Add(title);

        return Created($"/api/v1/books/{books.Count - 1}", title);
    }

    // PUT: api/v1/books/1?title=Algorithms
    [HttpPut("{id}")]
    public IActionResult Update(int id, string title)
    {
        if (id < 0 || id >= books.Count)
            return NotFound("Book not found.");

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Book title is required.");

        books[id] = title;

        return Ok(books[id]);
    }

    // DELETE: api/v1/books/1
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id < 0 || id >= books.Count)
            return NotFound("Book not found.");

        books.RemoveAt(id);

        return NoContent();
    }
}