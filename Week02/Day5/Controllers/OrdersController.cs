using Microsoft.AspNetCore.Mvc;
using MiddlewareDemo.Services;

namespace MiddlewareDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_orderService.GetOrders());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(_orderService.GetOrderById(id));
    }

    [HttpPost("{customerName}")]
    public IActionResult Create(string customerName)
    {
        return Ok(_orderService.CreateOrder(customerName));
    }
}