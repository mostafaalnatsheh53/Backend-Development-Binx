using Day1.Data;
using Day1.DTOs;
using Day1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Day1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    // GET: api/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    // GET: api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.ToListAsync();

        return Ok(orders);
    }

    // PUT: api/orders/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        order.CustomerId = request.CustomerId;

        await _context.SaveChangesAsync();

        return Ok(order);
    }

    // Only Admin users can delete orders.
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}