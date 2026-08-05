using Day4.Data;
using Day4.DTOs;
using Day4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Day4.Controllers;

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
    //create a new order
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
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);




    }
    [HttpGet("{id}")]
    // GET: api/orders/{id}
    //get order by id
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

    [HttpGet]
    // GET: api/orders
    //get all orders
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.ToListAsync();

        return Ok(orders);
    }
    [HttpPut("{id}")]
    // PUT: api/orders/{id}
    //update order by id
    public async Task<IActionResult> Update(int id, UpdateOrderRequest request)
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