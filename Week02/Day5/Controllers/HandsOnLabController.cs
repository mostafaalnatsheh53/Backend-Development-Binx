using MiddlewareDemo.Services;
using Microsoft.AspNetCore.Mvc;
namespace Day5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HandsOnLabController : ControllerBase
{
    private readonly IOrderService _orderService;

    public HandsOnLabController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(_orderService.GetOrders());
    }
}